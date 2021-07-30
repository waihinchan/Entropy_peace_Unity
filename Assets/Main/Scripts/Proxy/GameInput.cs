using System;
using UnityEngine;


public class GameInput
{
    public void HeartBeats(object data, UserManager manager, FuncCode funcCode)
    {
        manager.ProxyManager.Call(FuncCode.HeartBeatsBack, new HeartBeatsBack());
    }

    public void HeartBeatsBack(object data, UserManager manager, FuncCode funcCode)
    {
        manager.ProxyManager.timeBack = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0))
            .TotalSeconds);
    }

    // 客机请求主机开始游戏
    public void GiveUserInfo(object data, UserManager manager, FuncCode funcCode)
    {
        var userInfo = (GiveUserInfo)(data); 
        manager.MasterGameStart(userInfo.UserName);
    }
    
    // 主机回复客机，开始游戏
    public void GiveGameInfo(object data, UserManager manager, FuncCode funcCode)
    {
        var gameInfo = (GiveGameInfo)(data);
        manager.SlaverGameStart(gameInfo);
    }
    
    public void Settle(object data, UserManager manager, FuncCode funcCode)
    {
        manager.TriggerCall(funcCode, data);
    }
    
}