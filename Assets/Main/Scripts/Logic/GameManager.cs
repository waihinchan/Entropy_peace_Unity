using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public ChessBoard ChessBoard; //chessboard去获得player的信息,然后生成棋子在上面.
    private Player MyPlayer;
    private Player OpPlayer;
    private UserManager UserManager;
    private GameInfo gameInfo;
    private MainPlayer _mainPlayer;
    private Game _game;

    private GameContext gameContext
    {
        get
        {
            return _game.GameContext;
        }
    }
    
    public bool IsGameEnd
    {
        get
        {
            if (gameContext.Pollution >= gameInfo.TotalPollution)
            {
                return true;
            }

            if (gameContext.TurnCount > gameInfo.TotalRound)
            {
                return true;
            }

            return false;
        }
    }

    public void Start()
    {
        gameInfo = UserManager.GameInfo;
        
        _game = GetComponent<Game>();
        UserManager = GameObject.Find("Manager").GetComponent<UserManager>();
        _mainPlayer = GameObject.Find("MainPlayer").GetComponent<MainPlayer>();
        MyPlayer = new Player(UserManager.MyUserInfo.UserName);
        OpPlayer = new Player(UserManager.OpUserInfo.UserName);
    }
    
    public void GameOver()
    {
        // 判断输赢结算等逻辑
    }
    
    // 当结束回合时调用这个函数
    public void StopMainPlayerTurn()
    {
        var builderList = _mainPlayer.SubmitBuilderList();
        MySettle(builderList);
    }
    
    void Update()
    {
        if (gameContext == null)
        {
            Debug.Log("游戏未开始");;
            return;
        }

        if (!gameContext.IsMyTurn)
        {
            return;
        }
        
        gameContext.TurnTime += Time.deltaTime;
        
        if (gameContext.TurnCount >= gameInfo.EachRoundTime)
        {
            StopMainPlayerTurn();
        }
    }

    public void BigTurnEnd(){ 
        MyPlayer.SubmitAllValue();
        OpPlayer.SubmitAllValue();
        gameContext.OpDone = false;
        gameContext.MyDone = false;
        gameContext.TurnTime = 0;
        gameContext.TurnCount += 1;

        if (IsGameEnd)
        {
            Debug.Log("Game over!!!");
            GameOver();
        }
    }
    
    public void SmallTurnEnd()
    {
        if (gameContext.OpDone && gameContext.MyDone)
        {
            BigTurnEnd();
        }
        gameContext.IsMyTurn = !gameContext.IsMyTurn;
        gameContext.TurnTime = 0;
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
        gameContext.MyDone = true;
        var settle = new Settle();
        settle.ChessList = GameUtil.ConvertChessList(turnChessList);
        UserManager.ProxyManager.Call(FuncCode.Settle, JsonUtility.ToJson(settle));

        SmallTurnEnd();
    }

    public void OpSettle(List<ValueTuple<string, ValueTuple<int,int>>> turnChessNameList)
    {
        var turnChessList = GameUtil.ConvertChessList(turnChessNameList);
        SettleChess(turnChessList, OpPlayer);
        gameContext.OpDone = true;
        
        SmallTurnEnd();
    }

}
