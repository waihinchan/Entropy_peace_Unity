
using System;
using System.Collections.Generic;

public enum FuncCode
{
    HeartBeats,
    HeartBeatsBack,
    GiveUserInfo,
    GiveGameInfo,
    Settle,
}

[Serializable] 
public class Empty
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
}

[Serializable] 
public class Settle
{
    public List<ValueTuple<string,ValueTuple<int,int>>> ChessList;
}