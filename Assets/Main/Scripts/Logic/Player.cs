using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{
    public string name{get;set;} //玩家名字
    public Dictionary<string,float> EachRoundInfo = new Dictionary<string, float>();
    public List<Chess> OwnChess = new List<Chess>();

    public Player(string name)
    {
        this.name = name;
        EachRoundInfo[ConstantString.CurrentGeneratePollution]= 0; 
        EachRoundInfo[ConstantString.CurrentGenerateGold] = 0;
        EachRoundInfo[ConstantString.CurrentOwnGold] = 0; 

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
        EachRoundInfo[ConstantString.CurrentGeneratePollution] = 0;
        EachRoundInfo[ConstantString.CurrentGenerateGold] = 0;
        foreach(Chess chess in OwnChess){
            EachRoundInfo[ConstantString.CurrentGeneratePollution]+= chess.FactoryType.GenPollution; //
            EachRoundInfo[ConstantString.CurrentGenerateGold] += chess.FactoryType.GenGold;
        }
        EachRoundInfo[ConstantString.CurrentOwnGold] += EachRoundInfo[ConstantString.CurrentGenerateGold]; //更新自己拥有的金币信息
    }

}
