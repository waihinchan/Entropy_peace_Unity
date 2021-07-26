using System;
using UnityEngine;

public class Chess: MonoBehaviour
{
    public FactoryType FactoryType{get; set;} // 这个是棋子类,和棋盘分开.
    public Player Owner{get; set;} //有一个唯一id;
    
    public ValueTuple<int,int> Index;

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void InitChess(FactoryType factoryType, ValueTuple<int,int> index, Player owner){
        FactoryType = factoryType;
        Index = index;
        Owner = owner;
    }
}