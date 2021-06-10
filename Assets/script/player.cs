using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    

public class playerscript : MonoBehaviour
{ 
    // Start is called before the first frame update
    public static float init_power;
    public static float inmit_resource;
    public float resource ; //后面改成getset
    public float electricity ;
    public float total_pollution;
    public float population ;
    public bool interactable ;
    public bool is_AI ;
    private List<GameObject> owning_chesses; //这个是玩家当前拥有的建筑
    private Factory_type slot1;  // slot 1234 后期可以写成一个list..现在就算了
    private Factory_type slot2;
    public List<GameObject> Owning_chesses{
        get{return owning_chesses;}
        set{owning_chesses = value;}
    }
    public Factory_type Slot1{
        get{return slot1;}
        set{slot1 = value;}
    }
    public Factory_type Slot2{
        get{return slot2;}
        set{slot2 = value;}
    }
    //基本上用来传数据而已
    void Awake(){ //please usre awake
        resource = inmit_resource;
        electricity = init_power;
        total_pollution = 0;
        population = 0;
        interactable = false;
        owning_chesses = new List<GameObject>();
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public float settle(){
        float temp_pollution = 0;
        if(owning_chesses.Count!=0){
            foreach (GameObject chess in owning_chesses)
            {   
                chess tempchess = chess.GetComponent<chess>();
                population+=tempchess.population;
                electricity+=tempchess.energy;
                resource+=tempchess.resource;
                temp_pollution+=tempchess.pollution;
                if(electricity<0){
                    electricity = 0;
                }
                if(resource<0){
                    resource = 0;
                }
            }
            total_pollution+=temp_pollution;
        }

        return temp_pollution;
    }
}
