using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init_scene : MonoBehaviour
{   public SceneInitClass sceneinit; 
    private List<GameObject> chess_list;
    // Start is called before the first frame update
    void Start()
    {      
        // get the size to put it one by one
        Renderer rend;
        GameObject chess_board = sceneinit.chess_unit.transform.GetChild(1).gameObject;
        rend = chess_board.GetComponent<Renderer>();
        Vector3 unit_size = rend.bounds.size;
        int row = (int)Mathf.Sqrt(sceneinit.checkerboards);
        int col = row;
        Vector3 unit_position;
        // get the size to put it one by one
        chess_list = new List<GameObject>();
        for(int i = 0; i < row; i++){
            for(int j = 0; j < col; j++){
                Vector3 current_index = new Vector3((float)i*sceneinit.interval,0,(float)j*sceneinit.interval);
                unit_position = Vector3.Scale(unit_size , current_index); 
                // Debug.Log(unit_position);
                GameObject chess = Instantiate(sceneinit.chess_unit);
                chess.transform.position = unit_position;
                chess_list.Add(chess);
                
            }      
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}






