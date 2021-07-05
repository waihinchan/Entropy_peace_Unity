using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//room的作用是计算,同时接收客户端的信息
public partial class Room : MonoBehaviour
{   
    public ChessBoard myChessBoard; //chessboard去获得player的信息,然后生成棋子在上面.
    public bool gameStart{get;set;} = false;
    bool isHost; //如果这个是主机,直接找到这个主机的所在的另外一个脚本去更新东西
    public bool turn_end{get;set;} //这个是玩家主动结束回合,暂时用一个公共变量.动态去修改就可以了.
    public int StartCountDown{get;set;} //这个是开始游戏的倒计时
    public float turnCountDown{get;set;} //这个是小回合的倒计时
    public RoomState CurrentState{get;set;} //当前玩家状态
    public Player CurrentPlayer{get;set;} //这个是当前小回合的玩家.
    public bool game_end{get;set;} //游戏是否结束
    public int CurrentTurn{get;set;} //剩余回合数
    public int TotalTurns{get;set;} //总共回合数
    public Dictionary<int,Player> playerList{get;set;} //这个是跟客户端ID和对方的ID,用字典/哈希表来管理.
    public RoomInitInfo roomInitInfo = null; //这个用于快速测试,后期需要把这个部分到后台或者客户端.
    public float totalPollution{get;set;} //总生命值
    public float currentPollution{get;set;} //当前生命值
    Myself myself;
    public bool SlotEmpty(int chessbassindex){
        return myChessBoard.chessBasses[chessbassindex].GetComponent<Chessbass>().owner == null;
    }
    public void sendInfoToPlayer(int id,bool isTurn){
        if(roomInitInfo.Debug){
            if(playerList.ContainsKey(id)){
                playerList[id].myTurn = isTurn;
                Debug.Log($"send the turn info to the current player {playerList[id].name}");
            }
        }
    }
    void Awake(){
        assignRoominfo(roomInitInfo);
    }
    void assignRoominfo(RoomInitInfo _roomInitInfo){
        playerList = new Dictionary<int,Player>();
        if(roomInitInfo==null){
            roomInitInfo = _roomInitInfo; 
            //这个是在Debug模式下指派,如果是从机的话因为roomInitInfo是默认不为空的,所以并不需要指派变量
        }
        if(roomInitInfo==null){
            return; //如果是Debug模式下,awake运行到这一步就会弹出了,因为roomintinfo是空的.
        }
        isHost = roomInitInfo.isHost; //无论是加入房间还是创建房间都会有这个房间初始信息,但是我们只关注是否是主机.
        if(isHost){ //只有主机才对房间对象进行管理,如果不是房间只负责接收和提交信息,不做任何计算.
            init_room(roomInitInfo); //初始化房间信息,但是玩家列表还没有初始化完成,因为这里包含了等待玩家的状态
            //TODO:如果说等待界面和游戏内界面要分开两个secene的话,状态机需要转移到下一个场景.或者说把这个脚本保留为不销毁对象.
            CurrentState = new WaitingForStart(this); //初始化房间状态.这里包含了等待玩家进入和游戏过程的状态.
            this.StartCoroutine(CurrentState.StartState());
        }
#if UNITY_EDITOR
        else{
            if(roomInitInfo.Debug){
                Debug.Log("a guest join!");
            }
            // Getinfo();//这里不一定说是在这里更新房间信息.也可以是所有人齐了之后由主机发送一次GUI信息来更新.因为客户端不参与运算,所以这些信息只是用来显示
            //(但是玩家自己的信息是要做计算的,比如说建造条件这种.是要判断玩家当前拥有多少钱、拥有的建筑数量等/)
        }
#endif
        initMyself();//这个放在最后保证roominitinfo不为空,从而可以初始化牌库.

    }

    void initMyself(){ //初始化一些GUI之类的东西
        myself = gameObject.AddComponent<Myself>();
        myself.Deck = roomInitInfo.factory_types;
#if UNITY_EDITOR
        if(roomInitInfo.Debug){
            Debug.Log($"my deck account is {myself.Deck.Count}");
            foreach (Factory_Type singleFactory in myself.Deck)
            {
                Debug.Log(singleFactory.name);
            }
        }
#endif
    }
    void CheckGameStart(){
        if(playerList.Count==roomInitInfo.totalPlayer){
            gameStart = true;
        }
    }
    void Start(){}
    void Update()
    {   

        upDateRoom();
    }
    //running stage:
    void upDateRoom(){
        if(!gameStart){
            game_end = false;
            if(playerList.Count == roomInitInfo.totalPlayer){
                gameStart = true;
                Debug.Log("full people!");
            }
        }
#if UNITY_EDITOR
        if(roomInitInfo.Debug){
            DebugMode();
        }
#endif
        // 因为写了状态机,所以这里只是接收一些参数比如说接收来自其他玩家的信息并调用房间内的函数等.然后由状态机来维护.
    }
    void judgeGameEnd(){
        if(currentPollution<=0){ //一旦生命值归0游戏就结束. 其实这里应该是发生在状态机内或者回合结束后的,先这么写着.
            game_end = true;
        }
        // if(CurrentTurn==0){
        //     game_end = true;
        // }
    }
    public void Bigsetttle(){ //这个是大回合的结算的逻辑.
        foreach (Player singlePlayer in playerList.Values){
            singlePlayer.SubmitAllValue(); 
            //让每个玩家更新他们自己的当前污染、当前拥有金币(金币这些信息在函数内更新)等.污染写在外面是以防他们要改什么污染条件之类的东西可能需要知道其他玩家的信息
            currentPollution+=singlePlayer.EachRoundInfo[constantString.current_generate_pollution]; //更新总生命值
            singlePlayer.EachRoundInfo[constantString.grand_total_pollution]+=singlePlayer.EachRoundInfo[constantString.current_generate_pollution]; //累计自己产生的污染.
        }
        judgeGameEnd();
    }
    public void Smallsettle(){ //这个是小回合的结算逻辑,同样的如果需要所有玩家的其他信息来结算的话,就
        CurrentPlayer.SubmitAllValue(); 
        currentPollution-=CurrentPlayer.EachRoundInfo[constantString.current_generate_pollution]; 
#if UNITY_EDITOR
        if(roomInitInfo.Debug){
            Debug.Log($"current pollution is {currentPollution}");
    
        }
#endif
        judgeGameEnd();
    }
    public void stopMyTrun(){ //这个可以让玩家来调用.
        if(!turn_end){
            turn_end = true;
#if UNITY_EDITOR
            if(roomInitInfo.Debug){
                Debug.Log($"Current player {CurrentPlayer.name} just end turn in advanced!");
            }
#endif
        }
    }
    //running stage:

    //prepare stage
    void init_room(RoomInitInfo _roomInitInfo){
        totalPollution = _roomInitInfo.totalPollution;
        currentPollution = totalPollution;
        TotalTurns = _roomInitInfo.totalRound;
        CurrentTurn = TotalTurns;
        turnCountDown = _roomInitInfo.eachRoundTime;
        StartCountDown = _roomInitInfo.StartCountDown;
    }
    public void init_player(){
        if(playerList.Count>0 && roomInitInfo != null){
            int count = 0;
            foreach (Player singlePlayer in playerList.Values)
            {
                // singlePlayer.Deck = roomInitInfo.factory_types; //初始化牌库 这个移给myself
                singlePlayer.EachRoundInfo = new Dictionary<string,float>(); //初始化玩家拥有的信息
                singlePlayer.EachRoundInfo.Add(constantString.current_own_gold,roomInitInfo.initGold); //初始化拥有的金钱
                singlePlayer.EachRoundInfo.Add(constantString.grand_total_pollution,0);//初始化累计污染
                singlePlayer.ownChess = new List<Chess>(); //初始化建筑列表,这里可能还要再拓展一下,比如说建筑位置之类的,看看怎么封装
#if UNITY_EDITOR
                if(roomInitInfo.Debug){
                    if(count==playerList.Count-1){
                        singlePlayer.name = "Host";
                    }
                    else{
                        singlePlayer.name = count.ToString();
                    }
                    count++;
                }
#endif
            }
#if UNITY_EDITOR
            if(roomInitInfo.Debug){
                Debug.Log("init all players succeed!");
                sendplayertomyself();
            }
#endif
        }
        else{
            Debug.LogError("No player detect or roominfo missed!");
        }

    }
    //prepare stage



}
