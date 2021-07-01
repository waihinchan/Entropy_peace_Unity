using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour
{
    // Start is called before the first frame update

    public Factory_Type myFactory_Type{get;set;} // 这个是棋子类,和棋盘分开.
    public Player owner{get;set;} //有一个唯一id;
    Chessbass myChessbass; //所在棋盘的位置.
    public Chess(Factory_Type _myFactory_Type, Chessbass _myChessbass){
        myFactory_Type = _myFactory_Type;
        myChessbass = _myChessbass;
    }
    void detectClick(){
        showGUI();
    }
    void Start()
    {
        
    }
    void showGUI(){
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
