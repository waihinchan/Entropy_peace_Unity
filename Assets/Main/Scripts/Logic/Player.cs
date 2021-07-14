using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{
    public string name{get;set;} //玩家名字
    public Dictionary<string,float> EachRoundInfo = new Dictionary<string, float>();
    public List<Chess> ownChess = new List<Chess>();

    public Player(string name)
    {
        this.name = name;
    }

    public void AddChess(Chess chess)
    {
        ownChess.Add(chess);
    }
    
    public void RemoveChess(Chess chess)
    {
        ownChess.Remove(chess);
    }
    

    // 结算数据
    public void SubmitAllValue(){ //每回合调用一次更新自己的信息
        EachRoundInfo[ConstantString.CurrentGeneratePollution] = 0;
        EachRoundInfo[ConstantString.CurrentGenerateGold] = 0;
        foreach(Chess chess in ownChess){
            EachRoundInfo[ConstantString.CurrentGeneratePollution]+= chess.FactoryType.GenPollution; //
            EachRoundInfo[ConstantString.CurrentGenerateGold] += chess.FactoryType.GenGold;
        }
        EachRoundInfo[ConstantString.CurrentOwnGold] += EachRoundInfo[ConstantString.CurrentGenerateGold]; //更新自己拥有的金币信息
    }

}
