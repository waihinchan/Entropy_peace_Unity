
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer
{
    public Dictionary<ValueTuple<int, int>, FactoryType> ChoiceBuilderList = new Dictionary<(int, int), FactoryType>();
    private Player player;
    private UserManager userManager;
    
    private void Start()
    {
        userManager = GameObject.Find("Manager").GetComponent<UserManager>();
    }

    public void Remove(ValueTuple<int, int> index)
    {
        FactoryType factoryType;
        var ok = ChoiceBuilderList.TryGetValue(index, out factoryType);
        if (!ok)
        {
            return;
        }

        ChoiceBuilderList.Remove(index);
    }
}