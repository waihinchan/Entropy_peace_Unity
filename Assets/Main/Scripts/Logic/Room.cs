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
    public bool game_end{get;set;} = false; //游戏是否结束
    public int CurrentTurn{get;set;} //剩余回合数
    public int TotalTurns{get;set;} //总共回合数
    public Dictionary<int,Player> playerList{get;set;} //这个是跟客户端ID和对方的ID,用字典/哈希表来管理.
    public float totalPollution{get;set;} //总生命值
    public float currentPollution{get;set;} //当前生命值
    Myself myself;
    // 这些是从上一个场景传送过来的信息
    string myname;
    string myip;
    public RoomInitInfo roomInitInfo = null; //注意这里的roomInitInfo.isHost需要从上一个场景中设置好再传过来.
    // 这些是从上一个场景传送过来的信息

    //******************************************utils******************************************
    public bool SlotEmpty(int chessbassindex){ 
        //注意,这个建造并不需要发起通讯,因为主机从机都会接收到所有棋盘上的信息,在机内自己判断就可以了
        return myChessBoard.chessBasses[chessbassindex].GetComponent<Chessbass>().owner == null;
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
    //******************************************utils******************************************
    void Awake(){
     //这里需要从上一个场景中获取到的房间初始化信息,然后在Start中去进行初始化.同时这里还需要获取这个客户端的玩家信息,名字IP等
     //GetInfoFromPreviousScene();
        assignRoominfo(roomInitInfo);
    }
    
    void Start(){}
    void Update(){upDateRoom();}
    //*****************************************************running stage*****************************************************
    void upDateRoom(){
        if(isHost){
            // ReceiveSingleRoomInfoFromGuest(); //这个是无时无刻在进行的. 因为对方的操作时机不确定
        }
        else{
            // ReceiveSingleRoomInfoFromHost(); 
            // GetControlFromHost();
        }
#if UNITY_EDITOR
        if(roomInitInfo.Debug){
            DebugMode();
        }
#endif
        // 因为写了状态机,所以这里只是接收一些参数比如说接收来自其他玩家的信息并调用房间内的函数等.然后由状态机来维护.
    }
    void judgeGameEnd(){
        if(currentPollution<=0){ 
            //一旦生命值归0游戏就结束.
            //TODO :如果游戏结束的话,需要发送数据到从机(虽然从机也可以获取生命值的信息,从而在从机那里自己判断,但是这个函数只在状态机里面调用的,所以直接发送过去告诉从机游戏结束了就可以了)
            game_end = true;
        }
    }
    public void Bigsetttle(){ //这个是大回合的结算的逻辑. 现阶段没有用到
        foreach (Player singlePlayer in playerList.Values){
            singlePlayer.SubmitAllValue(); 
            //让每个玩家更新他们自己的当前污染、当前拥有金币(金币这些信息在函数内更新)等.污染写在外面是以防他们要改什么污染条件之类的东西可能需要知道其他玩家的信息
            currentPollution+=singlePlayer.EachRoundInfo[constantString.current_generate_pollution]; //更新总生命值
            singlePlayer.EachRoundInfo[constantString.grand_total_pollution]+=singlePlayer.EachRoundInfo[constantString.current_generate_pollution]; //累计自己产生的污染.
        }
        judgeGameEnd();
    }
    public void Smallsettle(){
        //这个是小回合的结算逻辑
        CurrentPlayer.SubmitAllValue(); 
        currentPollution-=CurrentPlayer.EachRoundInfo[constantString.current_generate_pollution]; 
#if UNITY_EDITOR
        if(roomInitInfo.Debug){
            Debug.Log($"current pollution is {currentPollution}");
    
        }
#endif
        SendSingleRoomInfo(); // 先把信息发过去.游戏结束在另外发.
        judgeGameEnd();
    }
    //*****************************************************running stage*****************************************************

    //*****************************************************prepare stage*****************************************************
    //这里是按照执行顺序依次往下写的
    //首先主机加入的时候需要根据我们设定好的初始信息来初始化房间信息.
    //同时自己也拥有名字和唯一ID(由IP地址之类的分配一个,或者随机分配一个,反正IP和ID和玩家能对得上号就行)
    //此时playerList为空的,可以先添加一个new player是自己.可以调用PlayerJoin
    //此后所有IP连接上都会调用PlayerJoin
    //等到所有玩家的数量达到了roominitinfo里面的最大玩家人数时,状态机会初始化所有玩家的信息
    //然后此时主机需要发送一次信息给所有玩家.TODO 是否需要在所有玩家都接收到信息后给一个就绪指令然后游戏才开始倒计时?
    void assignRoominfo(RoomInitInfo _roomInitInfo){
        playerList = new Dictionary<int,Player>();
#if UNITY_EDITOR
        if(roomInitInfo==null){//这个是在Debug模式下指派,如果是从机的话因为roomInitInfo是默认不为空的,所以并不需要指派变量
            roomInitInfo = _roomInitInfo; 
        }
        if(roomInitInfo==null){
            return; //如果是Debug模式下,非主机awake运行到这一步就会弹出了,因为roomintinfo是空的.
        }
#endif
        isHost = roomInitInfo.isHost; //无论是加入房间还是创建房间都会有这个房间初始信息,但是我们只关注是否是主机.
        if(isHost){ //只有主机才对房间对象进行管理,如果不是房间只负责接收和提交信息,不做任何计算.
            init_room(roomInitInfo); 
            CurrentState = new WaitingForStart(this); //初始化房间状态.这里包含了等待玩家进入和游戏过程的状态.
            this.StartCoroutine(CurrentState.StartState());
        }
#if UNITY_EDITOR
        else{
            // NOTE 如果说非host需要进入游戏才能加入的话就是在这里把自己的名字和ID和IP发送给主机就可以了
            // Like: SendMyInfoToHost();
            if(roomInitInfo.Debug){
                Debug.Log("a guest join!");
            }
            // Getinfo();//这里不一定说是在这里更新房间信息.也可以是所有人齐了之后由主机发送一次GUI信息来更新.因为客户端不参与运算,所以这些信息只是用来显示
            //(但是玩家自己的信息是要做计算的,比如说建造条件这种.是要判断玩家当前拥有多少钱、拥有的建筑数量等/)
        }
#endif
        initMyself(); //这个放在最后保证roominitinfo不为空,然后先初始化牌库.
        // int hostID = 0;
        // if(isHost){PlayerJoin(hostID);} //这里看看主机的ID是怎么样获得的.
        //如果是非Host的话,myself是要等待主机把最终结果分配过来的.
        //其实主要问题就是在于我不太确定那个唯一ID怎么分配的,如果说只是随机那可以由从机来分配,但是如果说和IP什么的有关系的话,可能就需要主机来分配
        


    }

    void initMyself(){ 
        //初始化一些GUI之类的东西,这里myself还没有指派哪个object是自己.需要所有玩家就绪后按照ID来发送就可以了
        //有一个接收的信息写在proxy里面.
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
    void init_room(RoomInitInfo _roomInitInfo){
        totalPollution = _roomInitInfo.totalPollution;
        currentPollution = totalPollution;
        TotalTurns = _roomInitInfo.totalRound;
        CurrentTurn = TotalTurns;
        turnCountDown = _roomInitInfo.eachRoundTime;
        StartCountDown = _roomInitInfo.StartCountDown;
    }
    public void WaitngForPeople(){
        if(!gameStart){ //这个gameStart开始了之后就一直是True
            game_end = false;
            if(playerList.Count == roomInitInfo.totalPlayer){ 
                //满人即开始. TODO: 需要写一个接收玩家进入的信息.然后玩家进入了之后(即获取了玩家的名字和IP,就自动新建一个Player对象加入到玩家列表)
                //此时只需要初始化它的名字和唯一ID编号就可以了(使用PlayerJoin函数),
                //其余的信息如初始金币信息等需要等到所有玩家都加入了再一次性初始化. 参见init_player
                gameStart = true;
                if(roomInitInfo.Debug){Debug.Log("full people!");}
            }
        }
    }
    public void PlayerJoin(string _name, int _uniqueID){ 
        //这个是给主机等待每一个IP加入的时候调用的的,在准备阶段从机的玩家列表是空的,主机将所有信息都准备好发送给所有从机即可.
        //满人的时候游戏就会开始.
        //同时从机也可以调用这个来初始化一个新的Player.
        //TODO 这个是写在RoomState.WaitingForStart的UPdateState里面的. 但是在状态机里写的应该是接收到IP后调用这个函数.需要在状态机里面写一下.
        Player newplayer = new Player();
        newplayer.name = _name;
        newplayer.ID = _uniqueID;
        playerList.Add(newplayer.ID,newplayer);

    }

    public void init_player(){ 
        //这个已经写在状态机里面了.
        if(playerList.Count>0 && roomInitInfo != null){
            int count = 0;
            foreach (Player singlePlayer in playerList.Values)
            {
                // singlePlayer.Deck = roomInitInfo.factory_types; //初始化牌库 这个移给myself
                singlePlayer.EachRoundInfo = new Dictionary<string,float>(); //初始化玩家拥有的信息
                singlePlayer.EachRoundInfo.Add(constantString.current_own_gold,roomInitInfo.initGold); //初始化拥有的金钱
                singlePlayer.EachRoundInfo.Add(constantString.grand_total_pollution,0);//初始化累计污染
                singlePlayer.ownChess = new List<Chess>(); //初始化建筑列表,这里可能还要再拓展一下,比如说建筑位置之类的,看看怎么封装
                //TODO : 这个是通讯功能.意思是让从机调用assignMyself这个函数.只需要传一个ID就可以了
                //
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
    //*****************************************************prepare stage*****************************************************



}
