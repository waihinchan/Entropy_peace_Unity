using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Room Base Information", menuName = "Room/Room Base Information", order = 1)]
public class RoomInitInfo : ScriptableObject {
    
    //这个部分后面移植到游戏开始界面之类的来设置,保留两个版本是因为我们可以在引擎内直接测试.
    public float totalPollution = 100;
    public int totalRound = 15;
    public float eachRoundTime = 50; // secs
    public float initGold = 100;
    public List<Factory_Type> factory_types; //这个部分assing给玩家,因为不需要发牌了
    public int totalPlayer = 2;
    public bool Debug = true;
    public bool isHost = false;
    public int StartCountDown = 10;

}

//2021注释:这个是在unity里面可以快速新建一种类型的建筑.然后拖放到房间内就可以自动生成这一种类型的“牌”发给玩家.