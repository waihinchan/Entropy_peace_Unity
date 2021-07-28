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
    public Vector3 offset = new Vector3(-2.8f,2.25f,-3f);
    public Chess[][] ChessMatrix;

    private UserManager userManager;
    
    // -3 2.25 -3
    void PlaceChessBases(){
        float x = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * chessBass.transform.lossyScale.x;
        float y = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * chessBass.transform.lossyScale.y;
        float z = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * chessBass.transform.lossyScale.z;
        Vector3 singlesize = new Vector3(x,y,z);
        int rows = (int)Mathf.Floor(Mathf.Sqrt(totalChessBass));
        int cols = rows;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject singleChess = Instantiate(chessBass, new Vector3(i*singlesize.x + 0.1f * i ,0,j*singlesize.z + j * 0.1f) + offset, Quaternion.identity);
                var chess = singleChess.GetComponent<Chess>();
                chess.InitChess(null, (i,j), null);
                chess.isOrigin = true;
            }
        }
    }
    
    void Start()
    {
        InitChessBoard();
        PlaceChessBases(); //这里是游戏开始才初始化棋盘还是说直接进入这个场景就可以初始化棋盘还不确定
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
        float x = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * chessBass.transform.lossyScale.x;
        float y = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * chessBass.transform.lossyScale.y;
        float z = chessBass.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * chessBass.transform.lossyScale.z;
        Vector3 singlesize = new Vector3(x,y,z);

        var  newChessObj = Instantiate(factoryType.FactoryOutlook, new Vector3(index.Item1*singlesize.x + 0.1f * index.Item1 ,
            0,index.Item2*singlesize.z + index.Item2 * 0.1f) + offset, Quaternion.identity);

        var newChess = newChessObj.GetComponent<Chess>();
        newChess.InitChess(factoryType, index, owner);
        ChessMatrix[index.Item1][index.Item2] = newChess;
        owner.AddChess(newChess); 
        return newChess;
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
