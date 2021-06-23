using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//room的作用是计算,同时接收客户端的信息
public class Room : MonoBehaviour
{   
    // Dictionary
    bool isHost; //如果这个是主机,直接找到这个主机的所在的另外一个脚本去更新东西
    public bool turn_end{get;set;}
    public int StartCountDown{get;set;}
    public float turnCountDown{get;set;}
    public RoomState CurrentState{get;set;}
    public Player CurrentPlayer{get;set;} //这个是当前可以动的玩家.
    public bool game_end{get;set;}
    public int CurrentTurn{get;set;}
    public int TotalTurns{get;set;}
    // GetEnumerator
    public Dictionary<int,Player> playerList{get;set;} //这个是跟客户端ID和对方的ID,用哈希表来管理.
    public RoomInitInfo roomInitInfo; //这个用于快速测试,后期需要把这个部分到后台或者客户端.
    public bool gameEnd;
    private int totalRound; //总回合数
    private int currentRound; //已经发生的回合数
    private float totalPollution; //总生命值
    private float currentPollution; //当前生命值
    private Player currentPlayer; //这个用于在GUI中显示当前是哪一个玩家的回合,同时用于指派回合给下一个玩家,以此循环.
    private float eachRoundTime; //每一轮的秒数
    private float currentRoundTimeLeft; //当前回合的剩余时间(每回合结束后复位)

    void Awake(){

    }
    void Start()
    {   

    }
    void Update()
    {      
        UpDateRoom();
    }
    void UpDateRoom(){
        // 这里持续监听来自客户端的信息,同时更新秒数/时间等不受客户端信息控制的GUI.

    }

    void init_room(RoomInitInfo _roomInitInfo){
        totalPollution = _roomInitInfo.totalPollution;
        currentPollution = totalPollution;
        totalRound = _roomInitInfo.totalRound;
        currentRound = totalRound;
        eachRoundTime = _roomInitInfo.eachRoundTime;
        currentRoundTimeLeft = eachRoundTime;
        init_player(_roomInitInfo,playerList); //初始化所有玩家信息;
        //这里在写一个gamestart之类的东西
        
    }
    void init_player(RoomInitInfo _roomInitInfo, Dictionary<int,Player> _playerList){
        if(_playerList.Count>0){
            foreach (Player singlePlayer in _playerList.Values)
            {
                singlePlayer.Deck = _roomInitInfo.factory_types; //初始化牌库
                singlePlayer.grandTotalPollution = 0; //初始化累计污染
                singlePlayer.Gold = _roomInitInfo.initGold; //初始化金钱
                singlePlayer.EachRoundInfo = new Dictionary<string,float>(); //初始化字典
                singlePlayer.ownChess = new Dictionary<string,float>(); //初始化字典
            }
        }
        else{
            Debug.LogError("No player detect!");
        }

    }

//update room event and state
    private IEnumerator startroom(float waitTime)
    {
        while (!game_end)//当游戏还没有结束当时候
        {
            yield return new WaitForSeconds(waitTime);
            free_roll();//this is happend for the next round
            update_room();
            print("round end at" + Time.time);
        }
        update_room();

    }
    void update_room(){}
    void free_roll(){

    }
//update room event and state

}
