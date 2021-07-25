using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ChessBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public int totalChessBass = 36; //最大棋盘数,其实这里棋盘数量应该是和人数有关系的..
    public GameObject chessBass; //这个是棋盘的底座.
    public Transform offset;
    void Start()
    {   

        PlaceChessBases(); //这里是游戏开始才初始化棋盘还是说直接进入这个场景就可以初始化棋盘还不确定

    }

    void PlaceChessBases(){

        if(offset==null){
            offset = transform;
        }
        float x = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * chessBass.transform.lossyScale.x;
        float y = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * chessBass.transform.lossyScale.y;
        float z = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * chessBass.transform.lossyScale.z;
        Vector3 singlesize = new Vector3(x,y,z);
        int rows = (int)Mathf.Floor(Mathf.Sqrt(totalChessBass));
        int cols = rows;
        int count = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject singlechess = Instantiate(chessBass, new Vector3(i*singlesize.x + 0.1f * i ,0,j*singlesize.z + j * 0.1f) + offset.position, Quaternion.identity);
                count++;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
