using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//chess 做个小动画吧烦死了不然不知道选中了
public class chess : MonoBehaviour
{   
    // sceneinit.chess_unit.transform.GetChild(1).gameObject
    private GameObject owner; // this is kind of belong to someone or?
    private GameObject factory_look;
    private Mesh newmesh;
    //generate
    public float pollution = 0;
    public float population = 0;
    public float energy = 0;
    public float resource = 0;
    public Material mymaterial;
    public Material AImaterial;
    //generate
    //cost
    // public float cost_energy = 0;
    // public float cost_resource = 0;
    //cost
    
    public GameObject Owner{
        get{return owner;}
        set{owner = value;}
    }
    private bool visibile = false;
    // Start is called before the first frame update
    void Start()
    {
        factory_look = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        // factory_look.SetActive (false);//初始我们看不到任何内容
        
  
        // Debug.Log(factory);
    }
    void checkpicking(){ //write a event listener in the future
        // if(owner!=null){
        //     if(build.picking_chess==gameObject){
        //         build.picking_chess = null;
        //     }
        // }
    }
    // Update is called once per frame
    void Update()
    {
        // checkpicking();
        
    }
    void OnMouseDown() //这里应该是调用某个GUI
    {   

        showGUI();
    }
    void showGUI(){
        if(owner==null){
            build.picking_chess = gameObject;
        }
    }
    void change_room(){

    }
    public void get_build(Factory_type newbuilding,GameObject newowner){

        //这里是替换模型
        owner = newowner; // pass the owner to this.
        Debug.Log(owner);
        factory_look.GetComponent<MeshFilter>().mesh = newbuilding.factory_outlook;
        if(!newowner.GetComponent<playerscript>().is_AI){

            factory_look.GetComponent<MeshRenderer>().material = mymaterial;
        }
        else{
            factory_look.GetComponent<MeshRenderer>().material = AImaterial;
        }

        pollution = newbuilding.pollution;
        population = newbuilding.population;
        energy = newbuilding.energy;
        resource = newbuilding.resource;
        // factory_look.SetActive (true);
        
    }
}

