
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer
{
    private List<ValueTuple<FactoryType, ValueTuple<int, int>>> ChoiceBuilderList = new List<(FactoryType, (int, int))>();
    private Player player;
    private UserManager userManager;
    
    private void Start()
    {
        userManager = GameObject.Find("Manager").GetComponent<UserManager>();
    }

    public List<ValueTuple<FactoryType, ValueTuple<int, int>>> SubmitBuilderList()
    {
        var tmp = ChoiceBuilderList;
        ChoiceBuilderList = new List<(FactoryType, (int, int))>();
        return tmp;
    }

    public void AddBuilder(ValueTuple<FactoryType, ValueTuple<int, int>> builderTuple)
    {
        ChoiceBuilderList.Add(builderTuple);
    }

    public void Remove(ValueTuple<int, int> index)
    {
        foreach (var tuple in ChoiceBuilderList)
        {
            if (index == tuple.Item2)
            {
                ChoiceBuilderList.Remove(tuple);
                return;
            }
        }
    }
    
    public const int ChoiceFactoryNum = 3;
    // 能选择的棋子类型
    public FactoryType[] CanChoiceFactorys = new FactoryType[ChoiceFactoryNum];
    
    public void RefreshFactory()
    {
        var rd = new System.Random();
        List<FactoryType> factoryTypes = new List<FactoryType>();
        foreach (var factoryType in userManager.GameInfo.FactoryTypes)
        {
            factoryTypes.Add(factoryType);
        }
        for (int i=0; i < Math.Min(ChoiceFactoryNum, userManager.GameInfo.FactoryTypes.Count);i++)
        {
            var choiceId = rd.Next(0, factoryTypes.Count);
            var factoryType = factoryTypes[choiceId];
            CanChoiceFactorys[i] = factoryType;
            factoryTypes.Remove(factoryType);
        }
    }
}