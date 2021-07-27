using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ChessBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public int totalChessBass = 36; //最大棋盘数,其实这里棋盘数量应该是和人数有关系的..
    public GameObject chessBassA; //这个是棋盘的底座.
    public GameObject chessBassB; //这个是棋盘的底座.
    public Transform offset;
    void Start()
    {   

        PlaceChessBases(); //这里是游戏开始才初始化棋盘还是说直接进入这个场景就可以初始化棋盘还不确定

    }

    void PlaceChessBases(){

        if(offset==null){
            offset = transform;
        }
        float x = chessBassA.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * chessBassA.transform.lossyScale.x;
        float y = chessBassA.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * chessBassA.transform.lossyScale.y;
        float z = chessBassA.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * chessBassA.transform.lossyScale.z;
        Vector3 singlesize = new Vector3(x,y,z);
        int rows = (int)Mathf.Floor(Mathf.Sqrt(totalChessBass));
        int cols = rows;
        int count = 0;
        int countList = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {   
                Vector3 chessposition = new Vector3(i*singlesize.x + 0.0f * i  - 7.5f ,0,j*singlesize.z + j * 0.0f - 7.5f) + offset.position;
                if(countList%2==0){
                    GameObject singlechess = Instantiate(chessBassA, chessposition , Quaternion.identity);
                }
                else{
                    GameObject singlechess = Instantiate(chessBassB, chessposition , Quaternion.identity);
                }
                count++;
                countList++;
                if(count%6==0){
                    countList+=1;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
