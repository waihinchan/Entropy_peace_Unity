using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessbass : MonoBehaviour
{
    // Start is called before the first frame update
    Chess myChess;//这里和chess是互相引用
    public Player owner{get;set;} = null; //可能会有重复,后面把功能写完看看有没有必要都引用同一个玩家对象.
    public int index{get;set;} //在列表中的索引.
    void Awake(){
        
    }
    void Start()
    {
        
    }
    public GameObject buildFactoryOnTop(Factory_Type whichFactory,Player _owner){
        if(owner!=null){ //这里不知道为什么会出现重复建造的情况
            Debug.LogError("this chessbass is not empty! function condition error!");
        }
        owner = _owner;
        GameObject newchess = Instantiate(whichFactory.factory_outlook,  this.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
        // Chess tempchess = new Chess(whichFactory,this);
        newchess.transform.parent = transform;
        Chess tempchess = newchess.GetComponent<Chess>();
        tempchess.initparams(whichFactory,this,owner); //这个owner是为了区分颜色,当然也可以获取上级对象来获取owner,不过这里指派一下影响也不是很大
        return newchess;

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
