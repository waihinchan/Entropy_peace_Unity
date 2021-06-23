using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Single Factory Type", menuName = "Room/Factory", order = 1)]
public class Factory_Type : ScriptableObject {
    public string name;
    public string description;
    public float pollution = 0;
    public float population = 0;
    public float energy = 0;
    public float resource = 0;
    public float cost_energy = 0;
    public float cost_resource = 0;
    // public int maxnum = 5; //这个是决定了这一种牌有多少个 //因为没有发牌,所以就不需要了
    public Mesh factory_outlook; 
    public Sprite mySprite;   

}

//2021注释:这个是在unity里面可以快速新建一种类型的建筑.然后拖放到房间内就可以自动生成这一种类型的“牌”发给玩家.