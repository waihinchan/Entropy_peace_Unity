using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public ChessBoard ChessBoard; //chessboard去获得player的信息,然后生成棋子在上面.
    private Player MyPlayer;
    private Player OpPlayer;
    private UserManager UserManager;
    private GameInfo _gameInfo;
    private MainPlayer _mainPlayer;
    private Game _game;
    private Camera _camera;
    
    private Dictionary<string, FactoryType> _factoryTypes = new Dictionary<string, FactoryType>();
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
            if (gameContext.Pollution >= _gameInfo.TotalPollution)
            {
                return true;
            }

            if (gameContext.TurnCount > _gameInfo.TotalRound)
            {
                return true;
            }

            return false;
        }
    }

    public void Start()
    {
        _gameInfo = UserManager.GameInfo;

        foreach (var factoryType in _gameInfo.FactoryTypes)
        {
            _factoryTypes.Add(factoryType.name, factoryType);
        }
        
        _game = GetComponent<Game>();
        UserManager = GameObject.Find("Manager").GetComponent<UserManager>();
        _mainPlayer = GameObject.Find("MainPlayer").GetComponent<MainPlayer>();
        MyPlayer = new Player(UserManager.MyUserInfo.UserName);
        OpPlayer = new Player(UserManager.OpUserInfo.UserName);

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    
    public void GameOver()
    {
        // 判断输赢结算等逻辑
        
    }

    private FactoryType _selectFactoryType;
    public void SelectBuilder(string builderName)
    {
        var ok = _factoryTypes.TryGetValue(builderName, out _selectFactoryType);
        if (!ok)
        {
            Debug.Log("没有这个建筑");
            _selectFactoryType = null;
            return;
        }

        if (_selectFactoryType.CostGold > MyPlayer.EachRoundInfo[ConstantString.CurrentOwnGold])
        {
            _selectFactoryType = null;
            return;
        }
    }

    // 当结束回合时调用这个函数
    public void StopMainPlayerTurn()
    {
        var builderList = _mainPlayer.ChoiceBuilderList;
        List<ValueTuple<FactoryType, ValueTuple<int,int>>> turnChessList = new List<(FactoryType, (int, int))>();
        foreach (var keyValue in builderList)
        {
            turnChessList.Append((keyValue.Value, keyValue.Key));
        }
        MySettle(turnChessList);
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
        
        if (gameContext.TurnCount >= _gameInfo.EachRoundTime)
        {
            StopMainPlayerTurn();
        }

        if (Input.GetMouseButtonUp(0) && _selectFactoryType)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); //相机发射射线  
            RaycastHit hitInfo;
            bool isCollider = Physics.Raycast(ray, out hitInfo);
            if (isCollider)
            {
                if (hitInfo.transform.gameObject.GetComponent<Chess>() != null)
                {
                    var chess = hitInfo.transform.gameObject.GetComponent<Chess>();
                    if (chess.Owner != null)
                    {
                        _selectFactoryType = null;
                    }
                    else
                    {
                        MyPlayer.EachRoundInfo[ConstantString.CurrentOwnGold] -= _selectFactoryType.CostGold;
                        _mainPlayer.ChoiceBuilderList.Add(chess.Index, _selectFactoryType);
                    }
                }
            }
        }
    }

    public void BigTurnEnd(){ 
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
        MyPlayer.SubmitAllValue();
        
        // 需要转到字符串头才能传
        UserManager.ProxyManager.Call(FuncCode.Settle, new Settle(){
            ChessList = GameUtil.ConvertChessList(turnChessList)
        });
        SmallTurnEnd();
    }

    public void OpSettle(List<ValueTuple<string, ValueTuple<int,int>>> turnChessNameList)
    {
        var turnChessList = GameUtil.ConvertChessList(turnChessNameList);
        SettleChess(turnChessList, OpPlayer);
        gameContext.OpDone = true;
        OpPlayer.SubmitAllValue();

        SmallTurnEnd();
    }

}
