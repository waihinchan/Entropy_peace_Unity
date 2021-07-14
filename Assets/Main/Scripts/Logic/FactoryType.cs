
using UnityEngine;

[CreateAssetMenu(fileName = "FactoryType", menuName = "Game/FactoryType", order = 1)]
public class FactoryType : ScriptableObject {
    public string Name; //名字
    public string Description; //描述,如果后面要分开字段的话也可以分
    public float GenPollution = 0; //产生的污染
    public float GenGold = 0; 
    public float CostGold = 0;
    public float RecyleGold = 0; //拆除返还的金币
    public GameObject FactoryOutlook; //这里绑一个prefab之类的
}