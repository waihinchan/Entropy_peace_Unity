using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{
    public string name{get;set;} //玩家名字
    public float CurrentGeneratePollution = 0;
    public float CurrentGenerateGold = 0;
    public float CurrentOwnGold = 0;
    public List<Chess> OwnChess = new List<Chess>();

    public Player(string name)
    {
        this.name = name;
    }

    public void AddChess(Chess chess)
    {
        OwnChess.Add(chess);
    }
    
    public void RemoveChess(Chess chess)
    {
        OwnChess.Remove(chess);
    }
    
    // 结算数据
    public void SubmitAllValue(){ //每回合调用一次更新自己的信息
        CurrentGeneratePollution = 0;
        CurrentGenerateGold = 0;
        foreach(Chess chess in OwnChess){
            CurrentGeneratePollution += chess.FactoryType.GenPollution; //
            CurrentGenerateGold += chess.FactoryType.GenGold;
        }
        CurrentOwnGold += CurrentGenerateGold; //更新自己拥有的金币信息
    }

}
