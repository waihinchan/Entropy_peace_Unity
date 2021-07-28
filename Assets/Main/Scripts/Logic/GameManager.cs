using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    public ChessBoard ChessBoard; //chessboard去获得player的信息,然后生成棋子在上面.
    private Player MyPlayer;
    private Player OpPlayer;
    private UserManager _userManager;
    private GameInfo _gameInfo;
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

    private Text timeText;
    private Text moneyText;
    private Text everyMoneyText;
    private Text pollutionText;
    private Text everyPollytionText;
    private Text turnText;
    public void Start()
    {
        timeText = GameObject.Find("GUI/BottomStatus/CONTROLPANEL/我的名字").GetComponent<Text>();
        moneyText = GameObject.Find("GUI/TopStatus/金币信息BG/每回合生产金币信息").GetComponent<Text>();
        everyMoneyText = GameObject.Find("GUI/TopStatus/金币信息BG/当前拥有总金币").GetComponent<Text>();
        pollutionText = GameObject.Find("GUI/TopStatus/污染信息BG/每回合生产金币信息").GetComponent<Text>();
        everyPollytionText = GameObject.Find("GUI/TopStatus/污染信息BG/当前拥有总金币").GetComponent<Text>();
        turnText = GameObject.Find("GUI/BottomStatus/当前回合 (1)").GetComponent<Text>();
        _userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        
        _gameInfo = _userManager.GameInfo;
        
        foreach (var factoryType in _gameInfo.FactoryTypes)
        {
            _factoryTypes.Add(factoryType.name, factoryType);
        }
        
        _game = GetComponent<Game>();
        _userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        MyPlayer = new Player(_userManager.MyUserInfo.UserName);
        MyPlayer.EachRoundInfo[ConstantString.CurrentOwnGold] = _gameInfo.InitGold;
        OpPlayer = new Player(_userManager.OpUserInfo.UserName);
        OpPlayer.EachRoundInfo[ConstantString.CurrentOwnGold] = _gameInfo.InitGold;

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // 如果不是主机
        if (!_userManager.isMaster)
        {
            // 将相机逆向
            _camera.transform.position = new Vector3(0, 6, 6);
            _camera.transform.localEulerAngles = new Vector3(45, 180, 0);
        }
    }
    
    public void GameOver()
    {
        // 判断输赢结算等逻辑
        
    }

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
        Vector3 prePosition = new Vector3(0, 0, 0);
        if (_tmpFactory)
        {
            Destroy(_tmpFactory);
        }
        _tmpFactory = Instantiate(_selectFactoryType.FactoryOutlook, prePosition, Quaternion.identity);
    }

    // 当结束回合时调用这个函数
    public void StopMainPlayerTurn()
    {
        var builderList = ChoiceBuilderList;
        List<ValueTuple<FactoryType, ValueTuple<int,int>>> turnChessList = new List<(FactoryType, (int, int))>();
        foreach (var keyValue in builderList)
        {
            turnChessList.Append((keyValue.Value, keyValue.Key));
        }
        MySettle(turnChessList);
        builderList.Clear();
    }
    
    public Dictionary<ValueTuple<int, int>, FactoryType> ChoiceBuilderList = new Dictionary<(int, int), FactoryType>();

    private FactoryType _selectFactoryType;
    private GameObject _tmpFactory;
    private Chess _selectOriginChess;
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
        
        timeText.text = "我方回合 ： " + (int)(_gameInfo.EachRoundTime - gameContext.TurnTime) + "s";
        moneyText.text = "总 " + MyPlayer.EachRoundInfo[ConstantString.CurrentOwnGold];
        everyMoneyText.text = "+ " + MyPlayer.EachRoundInfo[ConstantString.CurrentGenerateGold] + " / 回合 ";
        pollutionText.text = "总 " + gameContext.Pollution;
        everyPollytionText.text = "+ " + MyPlayer.EachRoundInfo[ConstantString.CurrentGeneratePollution] + " / 回合 ";
        turnText.text = gameContext.TurnCount+"/"+_gameInfo.TotalRound;

        gameContext.TurnTime += Time.deltaTime;
        if (gameContext.TurnTime >= _gameInfo.EachRoundTime)
        {
            StopMainPlayerTurn();
        }

        if (_tmpFactory != null)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); //相机发射射线  
            RaycastHit hitInfo;
            bool isCollider = Physics.Raycast(ray, out hitInfo);
            if (isCollider)
            {
                if (hitInfo.transform.gameObject.GetComponent<Chess>() != null )
                {
                    var chess = hitInfo.transform.gameObject.GetComponent<Chess>();
                    if (chess.isOrigin)
                    {
                        _tmpFactory.transform.position = chess.transform.position + new Vector3(0,0.2f, 0);
                        _selectOriginChess = chess;
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonDown(0) && _selectFactoryType!=null && _tmpFactory != null)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); //相机发射射线  
            RaycastHit hitInfo;
            bool isCollider = Physics.Raycast(ray, out hitInfo);
            if (isCollider)
            {
                var chess = _selectOriginChess;
                if(chess != null)
                {
                    _tmpFactory.transform.position = chess.transform.position;
                    var newChess = _tmpFactory.transform.GetComponent<Chess>();
                    newChess.InitChess(_selectFactoryType, chess.Index, MyPlayer);
                    ChessBoard.ChessMatrix[chess.Index.Item1][chess.Index.Item2] = newChess;
                    MyPlayer.AddChess(newChess);
                    MyPlayer.EachRoundInfo[ConstantString.CurrentOwnGold] -= _selectFactoryType.CostGold;
                    ChoiceBuilderList.Add(chess.Index, _selectFactoryType);

                    _selectFactoryType = null;
                    _tmpFactory = null;
                    _selectOriginChess = null;
                    Destroy(chess.gameObject);
                    return;
                }
            }
            Destroy(_tmpFactory);
            _selectFactoryType = null;
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
        // SettleChess(turnChessList, MyPlayer);
        gameContext.MyDone = true;
        MyPlayer.SubmitAllValue();
        
        // 需要转到字符串头才能传
        _userManager.ProxyManager.Call(FuncCode.Settle, new Settle(){
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
