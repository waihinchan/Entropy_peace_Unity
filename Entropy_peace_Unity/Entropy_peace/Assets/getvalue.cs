using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class getvalue : MonoBehaviour
{
    // Start is called before the first frame update
    string temptext;
    void Awake(){

        temptext = gameObject.GetComponent<Text>().text;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void getmyvalue(){
        gameObject.GetComponent<Text>().text =  temptext + gameObject.transform.parent.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value.ToString();
    }
}
