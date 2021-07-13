using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess
{
    // Start is called before the first frame update

    public FactoryType FactoryType{get;set;} // 这个是棋子类,和棋盘分开.
    public Player Owner{get;set;} //有一个唯一id;
    
    public ValueTuple<int,int> Index; 
    
    public Chess(FactoryType factoryType, ValueTuple<int,int> index, Player owner){
        FactoryType = factoryType;
        Index = index;
        Owner = owner;
    }
}
