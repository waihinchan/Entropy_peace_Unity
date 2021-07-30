
using System;
using System.Collections.Generic;
using UnityEngine;

public enum FuncCode
{
    HeartBeats,
    HeartBeatsBack,
    GiveUserInfo,
    GiveGameInfo,
    Settle,
}

[Serializable] 
public class HeartBeats
{
}


[Serializable] 
public class HeartBeatsBack
{
}

[Serializable] 
public class GiveUserInfo
{
    public string UserName;
}

[Serializable] 
public class GiveGameInfo
{
    public string MasterUserName;
    public float TotalPollution = 100;
    public int TotalRound = 15;
    public float EachRoundTime = 50; // secs
    public float InitGold = 100;
    public List<string> FactoryTypesName; //这个部分assing给玩家,因为不需要发牌了
    public int FirstId = 0;
    public int Id = 0;   // 分配的id
}

[Serializable] 
public class Settle
{
    public List<ChessInfo> ChessList;
}

[Serializable] 
public class ChessInfo
{
    public int x;
    public int y;
    public string typeName;
}

public class Test{
    public static void main()
    {
        // Settle settle = new Settle();
        // var a = new List<ValueTuple<string, ValueTuple<int, int>>>();
        // a.Add(("ggg",(1,1)));
        // var ss = JsonUtility.ToJson(settle);
        // Console.WriteLine(ss);
    }
}