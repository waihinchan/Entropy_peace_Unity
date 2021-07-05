using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Single Factory Type", menuName = "Room/Factory", order = 1)]
public class Factory_Type : ScriptableObject {
    public string name; //名字
    public string description; //描述,如果后面要分开字段的话也可以分
    public float gen_pollution = 0; //产生的污染
    public float gen_gold = 0; 
    public float cost_gold = 0;
    public float recyle_gold = 0; //拆除返还的金币
    public GameObject factory_outlook; //这里绑一个prefab之类的
    

}

//2021注释:这个是在unity里面可以快速新建一种类型的建筑.然后拖放到房间内就可以自动生成这一种类型的“牌”发给玩家.