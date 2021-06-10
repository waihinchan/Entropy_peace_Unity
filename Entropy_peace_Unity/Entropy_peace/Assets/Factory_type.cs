using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "singlefactorytype", menuName = "room/factory", order = 1)]
public class Factory_type : ScriptableObject {
    public string name;
    public string description;
    public float pollution = 0;
    public float population = 0;
    public float energy = 0;
    public float resource = 0;
    public float cost_energy = 0;
    public float cost_resource = 0;
    public int maxnum = 5; //这个是决定了这一种牌有多少个
    public Mesh factory_outlook; 
    public Sprite mySprite;   

}

