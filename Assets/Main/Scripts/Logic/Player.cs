using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{ 
    
    private int ID; //这个作为客户端标识
    public string name{get;set;} //玩家名字
    public Dictionary<string,float> EachRoundInfo{get;set;}
    //这个是玩家每回合产生的金币+污染和当前拥有的金钱和产生的总污染,用于回合结算.这个是由玩家当前拥有的建筑来进行计算的.
    //key字段参考constantString.
    public List<Chess> ownChess{get;set;} //这个是玩家当前拥有的建筑.目前这里还要再拓展成一个新的类型.
    public void SubmitAllValue(){ //每回合调用一次更新自己的信息
        EachRoundInfo[constantString.current_generate_pollution] = 0;
        EachRoundInfo[constantString.current_generate_gold] = 0;
        foreach(Chess chess in ownChess){
            EachRoundInfo[constantString.current_generate_pollution]+= chess.myFactory_Type.gen_pollution; //
            EachRoundInfo[constantString.current_generate_gold] += chess.myFactory_Type.gen_gold;
        }
        EachRoundInfo[constantString.current_own_gold] += EachRoundInfo[constantString.current_generate_gold]; //更新自己拥有的金币信息
    }
    void stopMyTrun(){
        //这里发送一个请求调用room里面的stopMyTrun function.
        //这个后面转移到GUI的功能吧.
    }
}
