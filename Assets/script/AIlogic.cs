using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIlogic : MonoBehaviour
{
    // Start is called before the first frame update
    private Factory_type slot1;  
    private Factory_type slot2;
    public float rollcost;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log(gameObject.GetComponent<playerscript>().resource);
        Debug.Log(gameObject.GetComponent<playerscript>().electricity);
        slot1 = gameObject.GetComponent<playerscript>().Slot1;
        slot2 = gameObject.GetComponent<playerscript>().Slot2;
        if(slot1!=null){

            Debug.Log(slot1.name);
        }
        if(slot2!=null){

            Debug.Log(slot2.name);
        }
        // build 
        if(slot1!=null){
            if(slot1.name == "Agriculture" && judge(slot1) && slot1!=null){
                buildthatslot(slot1);
                gameObject.GetComponent<playerscript>().Slot1 = null;
                Debug.Log(slot1.name);
                
            }
            if(slot1.name == "Eletricity&heat" && judge(slot1) && slot1!=null){
                buildthatslot(slot1);
                gameObject.GetComponent<playerscript>().Slot1 = null;
               
            }
            if(slot1.name == "Industrial&manufacturing" && judge(slot1) && slot1!=null){
                buildthatslot(slot1);
                gameObject.GetComponent<playerscript>().Slot1 = null;
                
            }
        }

        if(slot2!=null){
            if(slot2.name == "Agriculture" && judge(slot2) && slot2!=null){
                buildthatslot(slot2);
                gameObject.GetComponent<playerscript>().Slot2 = null;
               
            }
            if(slot2.name == "Eletricity&heat" && judge(slot1) && slot2!=null){
                buildthatslot(slot2);
                gameObject.GetComponent<playerscript>().Slot2 = null;
               
            }
            if(slot2.name == "Industrial&manufacturing" && judge(slot1) && slot2!=null){
                buildthatslot(slot2);
                gameObject.GetComponent<playerscript>().Slot2 = null;
                
            }
        }




        if(slot1==null&&slot2==null){
                // gameObject.GetComponent<playerscript>().resource-=rollcost;
                gameObject.GetComponent<playerscript>().Slot1 = GameObject.Find("Main Camera").GetComponent<room>().roll(gameObject.GetComponent<playerscript>().Slot1);
                gameObject.GetComponent<playerscript>().Slot2 = GameObject.Find("Main Camera").GetComponent<room>().roll(gameObject.GetComponent<playerscript>().Slot2);
                Debug.Log("i reroll");
            
            // else{
                // Debug.Log("no money mother fucker");
            // }
        }

        if(slot1!=null&&slot2!=null){ //当两个都不为空
            if((slot1.name=="Purify town"||slot1.name=="New energy") && (slot2.name=="Purify town"||slot2.name=="New energy")){
                // if slot1 == oneofthem and slot2 == oneofthem
                // Debug.Log(slot1.name);
                // Debug.Log(slot2.name);
                // if(gameObject.GetComponent<playerscript>().resource>=rollcost){ //if have money to roll
                    // gameObject.GetComponent<playerscript>().resource-=rollcost;
                    gameObject.GetComponent<playerscript>().Slot1 = GameObject.Find("Main Camera").GetComponent<room>().roll(gameObject.GetComponent<playerscript>().Slot1);
                    gameObject.GetComponent<playerscript>().Slot2 = GameObject.Find("Main Camera").GetComponent<room>().roll(gameObject.GetComponent<playerscript>().Slot2);
                    // Debug.Log("i reroll");
                // }
                // else{
                    // Debug.Log("no money mother fucker");
                // }
            }
        }


    }
    private bool judge(Factory_type thatfactory){
        if(gameObject.GetComponent<playerscript>().resource>=thatfactory.cost_resource&&gameObject.GetComponent<playerscript>().electricity>=thatfactory.cost_energy){
            return true;
        }
        else{
            return false;
        }
    }
    void buildthatslot(Factory_type thatslot){
            gameObject.GetComponent<playerscript>().resource -= thatslot.cost_resource;
            gameObject.GetComponent<playerscript>().electricity -= thatslot.cost_energy;
            GameObject.Find("Main Camera").GetComponent<room>().AIbuild(thatslot);
            // Debug.Log("build a" + thatslot.name);
            // thatslot = null;
    }
    
    
    
    
    
    }
    