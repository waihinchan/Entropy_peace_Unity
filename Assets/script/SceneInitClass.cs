using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "room", menuName = "room/setting", order = 1)]
public class SceneInitClass : ScriptableObject 
{   

    public string objectName = "myroom";
    public int HP = 100;
    public int rounds = 7;
    public uint checkerboards = 25;
    public GameObject chess_unit;
    public float interval = 5.0f;
    public float countdown = 10.0f;
    public string player_name = "You";
    // public int player_num = 2; //so far we only have 2
    public List<Factory_type> factory_types;
    public float rollcost = 3;
    public float init_power = 100;
    public float inmit_resource = 10;

}




