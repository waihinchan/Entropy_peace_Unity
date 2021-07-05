using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ChessBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public int totalChessBass = 36; //最大棋盘数,其实这里棋盘数量应该是和人数有关系的..
    public GameObject chessBass; //这个是棋盘的底座.
    public Transform offset;
    public List<GameObject> chessBasses{get;set;} 
    //每个棋子都自己保留实例化,但是这里留一个列表作为引用,用来接收服务端的数据并更新到这里.
    //这些基本上就是棋盘需要拥有的信息.发送信息的话玩家会引用一个列表发送到服务端(或者说只发送一个索引)
    void Start()
    {   

        PlaceChessBases(); //这里是游戏开始才初始化棋盘还是说直接进入这个场景就可以初始化棋盘还不确定

    }
    public GameObject BuildChess(int index, Factory_Type whichFactory,Player owner){
        return chessBasses[index].GetComponent<Chessbass>().buildFactoryOnTop(whichFactory,owner);
    }
    void PlaceChessBases(){
        chessBasses = new List<GameObject>();
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
                GameObject singlechess = Instantiate(chessBass, new Vector3(i*singlesize.x ,0,j*singlesize.z) + offset.position, Quaternion.identity);
                Chessbass thechessbass = singlechess.GetComponent<Chessbass>(); //这里已经把脚本放进去预制件了
                thechessbass.initparams(count);
                chessBasses.Add(singlechess);
                count++;
            }
        }
    }
    // Update is called once per frame
    void PlaceChessShell(){//这里放置棋子的外壳(如果有的话)
    }
    void Update()
    {
        
    }
}
