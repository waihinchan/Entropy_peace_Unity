using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class passsetting : MonoBehaviour
{
    // Start is called before the first frame update
    public SceneInitClass sceneinit;
    public Text nametext;
    public Slider HPslider;
    public Slider Roundslider;
    public Slider chessesslider;
    public Slider intervalslider;
    public Slider coundownslider;
    public Slider rollcostslider;
    public Slider initpslider;
    public Slider initrcostslider;

    public Dropdown allfactorytypes;
    public Slider pollution;
    public Slider population;
    public Slider power;
    public Slider resourc;
    public Slider costpower;
    public Slider costresource;
    public Slider maxnum;
    void Awake(){


        // HPslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = HPslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.HP.ToString();
        // Roundslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Roundslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.rounds.ToString();
        // chessesslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = chessesslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.checkerboards.ToString();
        // intervalslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = intervalslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.interval.ToString();
        // coundownslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = coundownslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.countdown.ToString();
        // rollcostslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = rollcostslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.rollcost.ToString();
        // initpslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = initpslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.init_power.ToString();
        // initrcostslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = initrcostslider.transform.parent.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text + sceneinit.inmit_resource.ToString();


    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        nametext.text = sceneinit.player_name.ToString();
        HPslider.value =  sceneinit.HP;
        Roundslider .value=  sceneinit.rounds;
        chessesslider .value=  sceneinit.checkerboards;
        intervalslider.value = sceneinit.interval;
        coundownslider.value =  sceneinit.countdown;
        rollcostslider.value =  sceneinit.rollcost;
        initpslider.value =  sceneinit.init_power;
        initrcostslider.value = sceneinit.inmit_resource;
    }

    // Update is called once per frame
    void Update()
    {   
        
        sceneinit.HP = (int)HPslider.value;
        sceneinit.player_name = nametext.text;
        sceneinit.rounds = (int)Roundslider.value;
        sceneinit.checkerboards = (uint)chessesslider.value;
        sceneinit.interval = intervalslider.value;
        sceneinit.countdown = coundownslider.value;
        sceneinit.rollcost = rollcostslider.value;
        sceneinit.init_power = initpslider.value;
        sceneinit.inmit_resource = initrcostslider.value;
    }
    
}
