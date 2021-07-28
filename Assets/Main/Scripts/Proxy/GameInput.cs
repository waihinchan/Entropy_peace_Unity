using System;
using UnityEngine;


public class GameInput
{
    public void HeartBeats(object data, ProxyManager manager)
    {
        manager.Call(FuncCode.HeartBeatsBack, new HeartBeatsBack());
    }

    public void HeartBeatsBack(object data, ProxyManager manager)
    {
        manager.timeBack = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0))
            .TotalSeconds);
    }

    // 客机请求主机开始游戏
    public void GiveUserInfo(object data, ProxyManager manager)
    {
        var userInfo = (GiveUserInfo)(data);
        manager.userManager.MasterGameStart(userInfo.UserName);
    }
    
    // 主机回复客机，开始游戏
    public void GiveGameInfo(object data, ProxyManager manager)
    {
        var gameInfo = (GiveGameInfo)(data);
        manager.userManager.SlaverGameStart(gameInfo);
    }
    
    public void Settle(object data, ProxyManager manager)
    {
        var settle = (Settle)(data);
        var room = GameObject.Find("Game").GetComponent<GameManager>();
        if (room != null)
        {
            Debug.Log("调用失败");
            return;
        }
        room.OpSettle(settle.ChessList);
    }
    
}