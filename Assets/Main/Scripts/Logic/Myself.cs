using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Myself : MonoBehaviour
{   
    public List<Factory_Type> Deck{get;set;} //玩家拥有的牌库.
    // Start is called before the first frame update
    // 这里类似于Player类,但是是自己的操作,捆绑了所有和自己相关的内容例如建造,拆除,提交信息等.
    public Player myself; //这里的信息由主机来分配,即我是谁.然后所有的建造条件拆除条件计算都读取这个类来运算.
    public List<Chess> tempNewChess{get;set;} //这个是玩家这个回合下新建造的.如果要撤销就在这个列表里面查找.
    public bool myTurn = false; //只有这个玩家在自己的回合下才可以行动.
    void Awake(){

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    bool judgeBuild(float targetGold){
        //判断是否可以建筑的条件.如果符合就扣钱.然后外部再把我自己的信息发送出去.(因为还要更新建筑列表) 也可以让玩家提交一个建造的请求,在主机那里做运算.
        if (targetGold <= myself.EachRoundInfo[constantString.current_own_gold])
        {
            myself.EachRoundInfo[constantString.current_own_gold] -= targetGold;
        return true;
        }
        else{
            return false;
        }

    }

}
