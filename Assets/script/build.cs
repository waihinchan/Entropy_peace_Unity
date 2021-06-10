using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//这个是用于建造
public class build : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameObject current_player;
    public static GameObject picking_chess;
    private Factory_type current_slot;
    public Text pollution;
    public Text power;
    public Text resrouce;
    public Text powerout;
    public Text resrouceout;
    public Text population;
    public Text fname;
    public Text description;
    public Factory_type Current_slot{
        get{return current_slot;}
        set{current_slot = value;}
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   


        if(picking_chess!=null&&current_slot!=null&&
            current_player.GetComponent<playerscript>().resource>=current_slot.cost_resource&&
            current_player.GetComponent<playerscript>().electricity>=current_slot.cost_energy)
            {   
                if(picking_chess.GetComponent<chess>().Owner==null){
                    gameObject.GetComponent<Button>().interactable = true;
                }
                else{
                    gameObject.GetComponent<Button>().interactable = false;
                    picking_chess= null;
                }
            }
        else
            {   

                gameObject.GetComponent<Button>().interactable = false;
                
            }
        if(current_slot!=null){
                pollution.text = current_slot.pollution.ToString();
                resrouceout.text = current_slot.resource.ToString();
                resrouce.text = current_slot.cost_resource.ToString();
                powerout.text = current_slot.energy.ToString();
                power.text = current_slot.cost_energy.ToString();
                population.text = current_slot.population.ToString();
                fname.text = current_slot.name.ToString();
                description.text = current_slot.description.ToString();
        }
        else{
            pollution.text = null;
            resrouceout.text = null;
            powerout.text = null;
            power.text = null;
            population.text = null;
            description.text = null;
            resrouce.text = null;
            fname.text = "Empty";
        }

    }
    public void building(){
        //omg this mother fxxer

        // Debug.Log("build something");
        //cacualte the value
        current_player.GetComponent<playerscript>().resource -= current_slot.cost_resource;
        current_player.GetComponent<playerscript>().electricity -= current_slot.cost_energy;
        //cacualte the value
        // the picking_chess will be set from the mouse everytime
        picking_chess.GetComponent<chess>().get_build(current_slot,current_player); // pass to the chess board
        current_player.GetComponent<playerscript>().Owning_chesses.Add(picking_chess); // pass to the player
        // release the slots
        picking_chess = null;
        current_slot = null;
        }

}
