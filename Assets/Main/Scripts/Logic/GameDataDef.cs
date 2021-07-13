using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FactoryType", menuName = "Game/FactoryType", order = 1)]
public class FactoryType : ScriptableObject {
    public string Name; //名字
    public string Description; //描述,如果后面要分开字段的话也可以分
    public float GenPollution = 0; //产生的污染
    public float GenGold = 0; 
    public float CostGold = 0;
    public float RecyleGold = 0; //拆除返还的金币
    public GameObject FactoryOutlook; //这里绑一个prefab之类的
}

[CreateAssetMenu(fileName = "GameInformation", menuName = "Game/GameInformation", order = 1)]
public class GameInitInfo : ScriptableObject
{
    public float TotalPollution = 100;
    public int TotalRound = 15;
    public float EachRoundTime = 50; // secs
    public float InitGold = 100;
    public List<FactoryType> FactoryTypes; //这个部分assing给玩家,因为不需要发牌了
    public bool IsMyTurn;
}

[CreateAssetMenu(fileName = "LocalUserInfo", menuName = "User/LocalUserInfo", order = 1)]
public class LocalUserInfo: ScriptableObject
{
    public int GameMoney = 100;
    public string UserName = "unknown";
    public int HistoryWin;
    public int HistoryFail;
    public int HistoryTie;
}
