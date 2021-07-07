using System;
using UnityEngine;


public class GameInput
{
    public void HeartBeats(string data, ProxyManager manager)
    {
        var req = JsonUtility.FromJson<EmptyReq>(data);
        var backReq = JsonUtility.ToJson(new EmptyReq());
        manager.Call(FuncCode.HeartBeatsBack, backReq);
    }

    public void HeartBeatsBack(string data, ProxyManager manager)
    {
        manager.timeBack = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0))
            .TotalSeconds);
    }

}