using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{ 
    public List<Factory_Type> Deck{get;set;} //玩家拥有的牌库
    private int ID; //这个作为客户端标识
    public string name{get;set;}
    public float Gold{get;set;} //玩家的当前金钱,第一次会由客户端指派.
    public float grandTotalPollution{get;set;}//这个是玩家累计产生的污染
    public Dictionary<string,float> EachRoundInfo{get;set;}//这个是玩家每回合产生的金币+污染,用于回合结算.这个是由玩家当前拥有的建筑来进行计算的.其实就两个字段,但是如果后面要增加的话就用字典来管理吧.
    public Dictionary<string,float> ownChess{get;set;} //这个是玩家当前拥有的建筑.
}
