using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public const int ChessWidth = 6; //最大棋盘数

    // Start is called before the first frame update
    public int totalChessBass = 36; //最大棋盘数..
    public GameObject chessBass; //这个是棋盘的底座.
    public Transform offset;
    public Chess[][] ChessMatrix;

    private UserManager userManager;
    
    void Start()
    {
        // userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        InitChessBoard();
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
    
    void InitChessBoard()
    {
        ChessMatrix = new Chess[ChessWidth][];
        for (int i=0; i<ChessMatrix.Length; i++)
        {
            ChessMatrix[i] = new Chess[ChessWidth];
            for (int j = 0; j < ChessMatrix[i].Length; j++)
            {
                ChessMatrix[i][j] = null;
            }
        }
    }
    
    public Chess BuildChess(FactoryType factoryType, ValueTuple<int,int> index, Player owner)
    {
        var newChess = new Chess(factoryType, index, owner);
        ChessMatrix[index.Item1][index.Item2] = newChess;
        owner.AddChess(newChess);        
        return newChess;
    }
    
    // 需要写一个位置的映射
    // 显示棋子的位置
    public void ShowChess(Chess chess, Tuple<int,int> index)
    {
        GameObject newchess = Instantiate(chess.FactoryType.FactoryOutlook,  this.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
    }
    
    // 消隐棋子的位置
    public void UnShowChess()
    {
        
    }
    
    public void RemoveChess(ValueTuple<int, int> index)
    {
        if (ChessMatrix[index.Item1][index.Item2] != null)
        {
            var chess = ChessMatrix[index.Item1][index.Item2];
            var player = chess.Owner;
            player.RemoveChess(chess);
        }
    }
}
