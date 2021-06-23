using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Room Base Information", menuName = "Room/Room Base Information", order = 1)]
public class RoomInitInfo : ScriptableObject {
    
    //这个只是暂时用于测试,因为后面这些内容可能需要根据玩家的人数进行变化,现阶段只是用于即时测试
    public float totalPollution = 100;
    public int totalRound = 15;
    public float eachRoundTime = 50; // secs
    // public float rollCost = 3; //没有发牌就不需要这个了
    public float initGold = 100;
    public List<Factory_Type> factory_types; //这个部分assing给玩家,因为不需要发牌了
    public int MaxNum = 1; //这个之前是用来计算卡牌出现的几率,这里先这样.
    public int totalPlayer = 2;
    public bool Debug = true;

}

//2021注释:这个是在unity里面可以快速新建一种类型的建筑.然后拖放到房间内就可以自动生成这一种类型的“牌”发给玩家.