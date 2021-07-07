using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "LocalUserInfo", menuName = "User/LocalUserInfo", order = 1)]
public class LocalUserInfo
{
    public int GameMoney;
    public string UserName;
    public int HistoryWin;
    public int HistoryFail;
    public int HistoryTie;

    public LocalUserInfo(string userName)
    {
        UserName = userName;
        GameMoney = 0;
        HistoryWin = 0;
        HistoryTie = 0;
        HistoryFail = 0;
    }
}

public class UserManager : MonoBehaviour
{
    private LocalUserInfo userInfo;
    private ClientConfig proxyConfig;
    private ProxyManager proxyManager;
    private void Start()
    {
        proxyManager = GetComponent<ProxyManager>();
    }

    public void CreateRoom()
    {
        proxyManager.StartServer();
        // ui跳转
    }

    public void JoinRoom(string host)
    {
        proxyManager.StartClient(host);
        // ui跳转
    }
    
}

