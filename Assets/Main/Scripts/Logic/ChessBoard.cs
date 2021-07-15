using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public const int ChessWidth = 6; //最大棋盘数
    
    public GameObject chessBass; //这个是棋盘的底座
    public Transform offset;
    public Chess[][] ChessMatrix;

    private UserManager userManager;
    
    void Start()
    {
        userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        InitChessBoard();
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
    
    public void RemoveChess(Tuple<int, int> index)
    {
        if (ChessMatrix[index.Item1][index.Item2] != null)
        {
            var chess = ChessMatrix[index.Item1][index.Item2];
            var player = chess.Owner;
            player.RemoveChess(chess);
        }
    }
}
