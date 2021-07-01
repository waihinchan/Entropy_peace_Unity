using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessbass : MonoBehaviour
{
    // Start is called before the first frame update
    Chess myChess;//这里和chess是互相引用
    Player owner; //可能会有重复,后面把功能写完看看有没有必要都引用同一个玩家对象.
    public int index{get;set;} //在列表中的索引.
    void Awake(){
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void initparams(int _index){
        index = _index;
        owner = null;
        myChess = null;
    }
    public void placeChess(Chess _myChess){ //放棋子,比如说拖放的时候,射线检测到这个棋格,然后松手释放就把参数传递进来
        myChess = _myChess;
        owner = _myChess.owner;
    }
}
