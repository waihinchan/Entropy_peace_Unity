using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Mainpage : MonoBehaviour
{   
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnClick() {

    //    UnityEditor.EditorApplication.isPlaying = false;
        SceneManager.LoadScene("setting");
    }
}



