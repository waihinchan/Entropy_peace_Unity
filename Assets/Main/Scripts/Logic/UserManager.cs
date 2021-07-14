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

public class UserManager : MonoBehaviour
{
    // 游戏初始化数据，由主机指定
    public ClientConfig ProxyConfig;
    public GameInitInfo GameInitInfo;
    public LocalUserInfo LocalUserInfo;
    
    public ProxyManager ProxyManager;
    public UserInfo MyUserInfo;
    public UserInfo OpUserInfo;
    
    private void Start()
    {
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
                MasterUserName = LocalUserInfo.UserName,
                InitGold = GameInitInfo.InitGold,
                TotalPollution = GameInitInfo.TotalPollution,
                TotalRound = GameInitInfo.TotalRound,
                EachRoundTime = GameInitInfo.EachRoundTime,
            }));
        // ui跳转
        // SceneManager.LoadScene("");
    }
    
    // 客机开始游戏
    public void SlaverGameStart(GiveGameInfo gameInfo)
    {
        MyUserInfo = new UserInfo(LocalUserInfo.UserName);
        OpUserInfo = new UserInfo(gameInfo.MasterUserName);
        GameInitInfo = new GameInitInfo();
        List<FactoryType> factoryTypes = new List<FactoryType>();
        foreach (var typeName in gameInfo.FactoryTypesName)
        {
            factoryTypes.Add(GameUtil.ConvertStringToFactory(typeName));
        }
        GameInitInfo.FactoryTypes = factoryTypes;
        GameInitInfo.InitGold = gameInfo.InitGold;
        GameInitInfo.TotalPollution = gameInfo.TotalPollution;
        GameInitInfo.TotalRound = gameInfo.TotalRound;
        GameInitInfo.EachRoundTime = gameInfo.EachRoundTime;
        Debug.Log("当前为从机，跳转到下一界面");
        // ui跳转
        // SceneManager.LoadScene("");
    }
}

