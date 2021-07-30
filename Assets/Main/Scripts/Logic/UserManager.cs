using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInfo
{
    public string UserName;

    public UserInfo(string userName)
    {
        UserName = userName;
    }
}

public class GameInfo
{
    // 最大污染
    public float TotalPollution = 100;
    public int TotalRound = 15;
    public float EachRoundTime = 60; 
    public float InitGold = 100;
    public List<FactoryType> FactoryTypes; 
    public int FirstId = 0;
    public int Id = 0;
}

public delegate void CallNext(object obj);

public class UserManager : MonoBehaviour
{
    // 游戏初始化数据，由主机指定
    public ClientConfig ProxyConfig;
    public GameInitInfo GameInitInfo;
    public LocalUserInfo LocalUserInfo;
    public GameInfo GameInfo;
    public ProxyManager ProxyManager;
    public UserInfo MyUserInfo;
    public UserInfo OpUserInfo;

    public string Username;
    public string Host;
    
    public bool isMaster = false;
    private void Start()
    {
        GameInfo = new GameInfo();
        GameInfo.FactoryTypes = GameInitInfo.FactoryTypes;
        GameInfo.InitGold = GameInitInfo.InitGold;
        GameInfo.TotalPollution = GameInitInfo.TotalPollution;
        GameInfo.EachRoundTime = GameInitInfo.EachRoundTime;
        GameInfo.TotalRound = GameInitInfo.TotalRound;
        GameInfo.Id = 0;
        GameInfo.FirstId = GameInitInfo.FirstId;
        
        ProxyManager = new ProxyManager(this);
        if (GameInitInfo == null)
        {
            return;
        }
        foreach (var type in GameInitInfo.FactoryTypes)
        {
            GameUtil.RegisterType(type.Name, type);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private volatile string loadSceneName = "";
    public void Update()
    {
        if (loadSceneName!="")
        {
            SceneManager.LoadScene(loadSceneName);
            loadSceneName = "";
        }
    }

    public void CreateRoom()
    {
        if (LocalUserInfo.GameMoney < 20)
        {
            Debug.Log("未有足够的金钱参加比赛");
            return;
        }
        Username = GameObject.Find("Canvas/名字").GetComponent<InputField>().text;
        ProxyManager.StartServer();
        Debug.Log("房间启动中,等待加入");
    }

    public void JoinRoom()
    {
        Host = GameObject.Find("Canvas/Ip").GetComponent<InputField>().text;
        Username = GameObject.Find("Canvas/名字").GetComponent<InputField>().text;
        JoinRoom(Host, Username);
    }
    
    public void JoinRoom(string host, string username)
    {
        if (LocalUserInfo.GameMoney < 20)
        {
            Debug.Log("未有足够的金钱参加比赛");
            return;
        }
        ProxyManager.StartClient(host);
        Debug.Log("房间加入中");
        ProxyManager.Call(FuncCode.GiveUserInfo, 
            new GiveUserInfo()
        {
            UserName = username,
        });
    }
    
    public Dictionary<FuncCode, List<CallNext>> _callNextDict = new Dictionary<FuncCode, List<CallNext>>();

    public void RegisterCallNext(FuncCode funcCode, CallNext callNext)
    {
        List<CallNext> callNextList;
        bool ok = _callNextDict.TryGetValue(funcCode, out callNextList);
        if (!ok)
        {
            callNextList = new List<CallNext>();
            _callNextDict.Add(funcCode, callNextList);
        }
        callNextList.Add(callNext);
    }
    
    public void TriggerCall(FuncCode funcCode, object obj)
    {
        List<CallNext> callNextList;
        bool ok = _callNextDict.TryGetValue(funcCode, out callNextList);
        if (!ok)
        {
            return;
        }
        foreach (var callNext in callNextList)
        {
            callNext(obj);
        }
    }
    
    // 主机开始游戏
    public void MasterGameStart(string userName)
    {
        MyUserInfo = new UserInfo(Username);
        OpUserInfo = new UserInfo(userName);
        Debug.Log("当前为主机，跳转到下一界面");
        
        List<string> factoryTypes = new List<string>();
        foreach (var type in GameInitInfo.FactoryTypes)
        {
            factoryTypes.Add(GameUtil.ConvertFactoryToString(type));
        }
        // 此处可拓展
        ProxyManager.Call(FuncCode.GiveGameInfo, 
            new GiveGameInfo()
            {
                FactoryTypesName = factoryTypes,
                MasterUserName = Username,
                InitGold = GameInitInfo.InitGold,
                TotalPollution = GameInitInfo.TotalPollution,
                TotalRound = GameInitInfo.TotalRound,
                EachRoundTime = GameInitInfo.EachRoundTime,
                Id = 1,
                FirstId = GameInitInfo.FirstId,
            });
        isMaster = true;
        // ui跳转
        loadSceneName = "Game";
    }

    public void EnterScene(string sceneName)
    {
        loadSceneName = sceneName;
    }
    
    // 客机开始游戏
    public void SlaverGameStart(GiveGameInfo gameInfo)
    {
        MyUserInfo = new UserInfo(Username);
        OpUserInfo = new UserInfo(gameInfo.MasterUserName);
        GameInfo = new GameInfo();
        List<FactoryType> factoryTypes = new List<FactoryType>();
        foreach (var typeName in gameInfo.FactoryTypesName)
        {
            factoryTypes.Add(GameUtil.ConvertStringToFactory(typeName));
        }
        GameInfo.FactoryTypes = factoryTypes;
        GameInfo.InitGold = gameInfo.InitGold;
        GameInfo.TotalPollution = gameInfo.TotalPollution;
        GameInfo.EachRoundTime = gameInfo.EachRoundTime;
        GameInfo.TotalRound = gameInfo.TotalRound;
        GameInfo.Id = gameInfo.Id;
        GameInfo.FirstId = GameInfo.FirstId;
        Debug.Log("当前为从机，跳转到下一界面");
        isMaster = false;
        // ui跳转
        loadSceneName = "Game";
    }
}

