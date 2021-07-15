using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Main.Scripts.Proxy;
using UnityEngine;

public class ProxyManager
{
    public UserManager userManager;

    private GameInput inputServer;
    private Queue<ValueTuple<FuncCode, string>> outMsgQueue = new Queue<ValueTuple<FuncCode, string>>();
    private Thread sendThread;
    private Socket clientSocket;
    private Socket serverSocket;
    private ClientConfig proxyConfig;
    private string host;
    private int port;
    
    //消息处理器
    private MessageHandler msg = new MessageHandler();
    
    //定时器
    System.Timers.Timer timer = new System.Timers.Timer(1000);

    public ProxyManager(UserManager userManager)
    {
        this.userManager = userManager;
        this.proxyConfig = userManager.ProxyConfig;
    }

    ~ProxyManager()
    {
        sendThread.Abort();
        serverSocket.Close();
        CloseConnect();
    }
    
    public void Start()
    {
        inputServer = new GameInput();
        host = proxyConfig.Host;
        port = proxyConfig.Port;
        sendThread = new Thread(new ThreadStart(SendToOther));
        sendThread.Start();
    }
    
    public void StartServer()
    {
        Start();
        TryListen();
    }

    public void StartClient(string host)
    {
        this.host = host;
        Start();
        TryConnect();
        
        // 从机要负责检测网络
        timer.Elapsed += new System.Timers.ElapsedEventHandler(KeepHeart);
        timer.AutoReset = false;
        timer.Enabled = true;
    }

    public long timeGo = 0;
    public long timeBack = -1;
    public long timeDelay = -1;
    public void KeepHeart(object sender, System.Timers.ElapsedEventArgs e)
    {
        timeDelay = timeBack - timeGo;
        timeGo = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0))
            .TotalSeconds);
        var reqData = JsonUtility.ToJson(new Empty());
        Call(FuncCode.HeartBeats, reqData);
        timer.Start();
    }
    
    public void TryListen()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port); //设置IP与port
        serverSocket.Bind(ipEndPoint); //绑定IP与端口号
        serverSocket.Listen(proxyConfig.MaxConn);  
        serverSocket.BeginAccept(AcceptCallback, null); //开始监听客户端的连接
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        if (clientSocket != null)
        {
            return;
        }
        clientSocket = serverSocket.EndAccept(ar);
        clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
        Debug.Log("连接进客户端！！！");
    }

    private bool IsConnectComplete()
    {
        return clientSocket != null;
    }
    
    private void FunctionInvoke(string functionName, string data)
    {
        OnCall(functionName, data);
    }
    
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);

            msg.ReadMessage(count, FunctionInvoke);
        }
        catch (Exception e)
        {
            Debug.Log("[ReceiveCB]:" + e.Message);
        }
    }
    
    public bool TryConnect()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host, port);
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log("无法连接到主机！！" + e);
            return false;
        }

        return true;
    }
    
    public void Call(FuncCode funcName, string reqData)
    {
        outMsgQueue.Enqueue((funcName, reqData));
    }

    private void OnCall(string funcName, string data)
    {
        MethodInfo mi = inputServer.GetType().GetMethod(funcName);
        mi.Invoke(inputServer, new object[]{data, this});
    }
    
    public void CloseConnect()
    {
        if (clientSocket == null)
        {
            return;
        }
        clientSocket.Close();
        clientSocket = null;
    }

    public void SendToOther()
    {
        while (true)
        {
            while (outMsgQueue.Count > 0)
            {
                var requestInfo = outMsgQueue.Dequeue();
                if (requestInfo.Item2 == null)
                {
                    continue;
                }

                byte[] bytes = MessageHandler.PackData(requestInfo.Item1, requestInfo.Item2);
                try
                {
                    clientSocket.Send(bytes);
                    Debug.Log("调用rpc："+requestInfo.Item1+".参数"+requestInfo.Item2);
                }
                catch (Exception e)
                {
                    Debug.Log("掉线了");
                    CloseConnect();
                }
            }
            Thread.CurrentThread.Join(10);//阻止设定时间
        }
    }
}