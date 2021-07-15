using System;
using System.Collections.Generic;
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
    public float TotalPollution = 100;
    public int TotalRound = 15;
    public float EachRoundTime = 50; // secs
    public float InitGold = 100;
    public List<FactoryType> FactoryTypes; //这个部分assing给玩家,因为不需要发牌了
}

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
    
    private void Start()
    {
        GameInfo = new GameInfo();
        GameInfo.FactoryTypes = GameInitInfo.FactoryTypes;
        GameInfo.InitGold = GameInitInfo.InitGold;
        GameInfo.TotalPollution = GameInitInfo.TotalRound;
        GameInfo.EachRoundTime = GameInitInfo.EachRoundTime;
        GameInfo.TotalRound = GameInitInfo.TotalRound;
        
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

    public void CreateRoom()
    {
        if (LocalUserInfo.GameMoney < 20)
        {
            Debug.Log("未有足够的金钱参加比赛");
            return;
        }
        ProxyManager.StartServer();
        Debug.Log("房间启动中,等待加入");
    }

    public void JoinRoom()
    {
        var host = GameObject.Find("Canvas/Ip/Text").GetComponent<Text>().text;
        JoinRoom(host);
    }
    
    public void JoinRoom(string host)
    {
        if (LocalUserInfo.GameMoney < 20)
        {
            Debug.Log("未有足够的金钱参加比赛");
            return;
        }
        ProxyManager.StartClient(host);
        Debug.Log("房间加入中");
        ProxyManager.Call(FuncCode.GiveUserInfo, 
            JsonUtility.ToJson(new GiveUserInfo()
        {
            UserName = LocalUserInfo.UserName,
        }));
    }

    // public bool GameStart()
    // {
    //     return myUserInfo != null && opUserInfo != null;
    // }
    
    // 主机开始游戏
    public void MasterGameStart(string userName)
    {
        MyUserInfo = new UserInfo(LocalUserInfo.UserName);
        OpUserInfo = new UserInfo(userName);
        Debug.Log("当前为主机，跳转到下一界面");
        
        List<string> factoryTypes = new List<string>();
        foreach (var type in GameInitInfo.FactoryTypes)
        {
            factoryTypes.Add(GameUtil.ConvertFactoryToString(type));
        }
        // 此处可拓展
        ProxyManager.Call(FuncCode.GiveGameInfo, 
            JsonUtility.ToJson(new GiveGameInfo()
            {
                FactoryTypesName = factoryTypes,
                MasterUserName = LocalUserInfo.UserName,
                InitGold = GameInitInfo.InitGold,
                TotalPollution = GameInitInfo.TotalPollution,
                TotalRound = GameInitInfo.TotalRound,
                EachRoundTime = GameInitInfo.EachRoundTime,
            }));
        
        var reqData = JsonUtility.ToJson(new Empty());
        Debug.Log("心跳");
        ProxyManager.Call(FuncCode.HeartBeats, reqData);

        // ui跳转
        // SceneManager.LoadScene("");
    }
    
    // 客机开始游戏
    public void SlaverGameStart(GiveGameInfo gameInfo)
    {
        MyUserInfo = new UserInfo(LocalUserInfo.UserName);
        OpUserInfo = new UserInfo(gameInfo.MasterUserName);
        GameInfo = new GameInfo();
        List<FactoryType> factoryTypes = new List<FactoryType>();
        foreach (var typeName in gameInfo.FactoryTypesName)
        {
            factoryTypes.Add(GameUtil.ConvertStringToFactory(typeName));
        }
        GameInfo.FactoryTypes = factoryTypes;
        GameInfo.InitGold = gameInfo.InitGold;
        GameInfo.TotalPollution = gameInfo.TotalRound;
        GameInfo.EachRoundTime = gameInfo.EachRoundTime;
        GameInfo.TotalRound = gameInfo.TotalRound;
        
        var reqData = JsonUtility.ToJson(new Empty());
        Debug.Log("心跳");
        ProxyManager.Call(FuncCode.HeartBeats, reqData);
        ProxyManager.Call(FuncCode.GiveGameInfo, 
            JsonUtility.ToJson(new GiveGameInfo()
            {
                MasterUserName = LocalUserInfo.UserName,
                InitGold = GameInitInfo.InitGold,
                TotalPollution = GameInitInfo.TotalPollution,
                TotalRound = GameInitInfo.TotalRound,
                EachRoundTime = GameInitInfo.EachRoundTime,
            }));
        
        Debug.Log("当前为从机，跳转到下一界面");
        // ui跳转
        // SceneManager.LoadScene("");
    }
}

