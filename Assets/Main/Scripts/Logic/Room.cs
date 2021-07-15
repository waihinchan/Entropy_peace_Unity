using System;
using System.Collections.Generic;
using UnityEngine;
//room的作用是计算,同时接收客户端的信息

public class GameContex
{
    public bool MyDone;
    public bool OpDone;
    public bool IsMyTurn;
    // 当前回合经历时间
    public float TurnTime;
    // 回合数
    public int TurnCount;
    public int Pollution;
    public GameContex(bool isMyTurn)
    {
        IsMyTurn = isMyTurn;
        OpDone = false;
        MyDone = false;
        TurnTime = 0f;
    }
}

public class Room : MonoBehaviour
{   
    public ChessBoard ChessBoard; //chessboard去获得player的信息,然后生成棋子在上面.
    public Player MyPlayer;
    public Player OpPlayer;
    private UserManager UserManager;
    private GameContex gameContex;
    private GameInfo gameInfo;
    public void Start()
    {
        UserManager = GameObject.Find("Manager").GetComponent<UserManager>();
        MyPlayer = new Player(UserManager.MyUserInfo.UserName);
        OpPlayer = new Player(UserManager.OpUserInfo.UserName);
        gameInfo = UserManager.GameInfo;
        gameContex = new GameContex(gameInfo.Id == gameInfo.FirstId);
    }

    public bool IsGameEnd
    {
        get
        {
            if (gameContex.Pollution >= gameInfo.TotalPollution)
            {
                return true;
            }

            if (gameContex.TurnCount > gameInfo.TotalRound)
            {
                return true;
            }

            return false;
        }
    }
    
    private List<ValueTuple<FactoryType, ValueTuple<int, int>>> TempBuilderList;

    // 当结束回合时调用这个函数
    public void StopMyTurn()
    {
        MySettle(TempBuilderList);
        TempBuilderList.Clear();
    }
    
    void Update()
    {
        UpdateRoom();
    }

    void UpdateRoom(){
        if (gameContex == null)
        {
            Debug.Log("游戏未开始");;
            return;
        }

        if (!gameContex.IsMyTurn)
        {
            return;
        }
        
        gameContex.TurnTime += Time.deltaTime;
        
        if (gameContex.TurnCount >= gameInfo.EachRoundTime)
        {
            StopMyTurn();
        }
        
    }
    
    // 在我结算的回合是
    public void Bigsetttle(){ 
        MyPlayer.SubmitAllValue();
        OpPlayer.SubmitAllValue();
        gameContex.OpDone = false;
        gameContex.MyDone = false;
        gameContex.TurnTime = 0;
        gameContex.TurnCount += 1;

        if (IsGameEnd)
        {
            Debug.Log("Game over!!!");
            return;
        }
    }

    public void SettleChess(List<ValueTuple<FactoryType, ValueTuple<int,int>>> turnChessList, Player player)
    {
        foreach (var chessData in turnChessList)
        {
            ChessBoard.BuildChess(chessData.Item1, chessData.Item2, player);
        }
    }
    
    public void MySettle(List<ValueTuple<FactoryType, ValueTuple<int,int>>> turnChessList)
    {
        SettleChess(turnChessList, MyPlayer);
        gameContex.MyDone = true;
        var settle = new Settle();
        settle.ChessList = GameUtil.ConvertChessList(turnChessList);
        UserManager.ProxyManager.Call(FuncCode.Settle, JsonUtility.ToJson(settle));
        if (gameContex.OpDone && gameContex.MyDone)
        {
            Bigsetttle();
        }
        gameContex.IsMyTurn = false;
    }
    
    public void OpSettle(List<ValueTuple<string, ValueTuple<int,int>>> turnChessNameList)
    {
        var turnChessList = GameUtil.ConvertChessList(turnChessNameList);
        SettleChess(turnChessList, OpPlayer);
        gameContex.OpDone = true;
        if (gameContex.OpDone && gameContex.MyDone)
        {
            Bigsetttle();
        }
        gameContex.IsMyTurn = true;
    }

}
