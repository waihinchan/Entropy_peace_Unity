using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Roll : MonoBehaviour
{   

    private GameObject player;
    public GameObject Player{
        get{return player;}
        set{player = value;}
    }
    public float rollcost;
    void Start(){
        // current_player = new GameObject();
        gameObject.GetComponent<Button>().interactable = false;
    }
    void Update(){
        if(player!=null && rollcost!=null){
            if(player.GetComponent<playerscript>().resource<rollcost){
                gameObject.GetComponent<Button>().interactable = false;
            }
            else{
                gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }
    public void reroll(){
        if(player!=null){ // update the player's slot
        player.GetComponent<playerscript>().Slot1 = GameObject.Find("Main Camera").GetComponent<room>().roll(player.GetComponent<playerscript>().Slot1);
        player.GetComponent<playerscript>().Slot2 = GameObject.Find("Main Camera").GetComponent<room>().roll(player.GetComponent<playerscript>().Slot2);
        GameObject.Find("Main Camera").GetComponent<room>().updatebutton(); //please don't do that in the future...
        player.GetComponent<playerscript>().resource-=rollcost;

        }
    }
}
