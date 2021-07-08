using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Room : MonoBehaviour
{   
    public void sendCountDownInfo(){ //这个写在每一个倒计时里面,自行插入即可.

    }
    // 游戏中
    public void MoveControlToPlayer(int id,bool isTurn){
        //这里的插入点在RoomState.EachPlayerCountDown状态机的开始和结束.开始则指定某个玩家可以行动,结束则指定某个玩家不能行动.
        //现在这里的写法是维护主机内playerList的myturn布尔,然后需要发送一个数据到从机/主机(如果是主机就是直接发给自己,不知道能不能自己向自己的ip发送,如果不能就另外写一个条件做判断吧)
        //建议下面这一步无论如何都要执行,这样数据过来的时候可以做一些验证避免错误.
        if(playerList.ContainsKey(id)){
            // playerList[id].myTurn = isTurn; //这个移动到GetControlFromHost
            //TODO : 把发送数据写在这里,发送一个布尔值.因为这里带了玩家的ID,如果需要拓展玩家IP地址之类的可以去改Player类,然后在字典里面找就是了
#if UNITY_EDITOR
            if(roomInitInfo.Debug){
                Debug.Log($"send the turn info to the current player {playerList[id].name}");
            }
#endif
        }
        else{
            Debug.LogError("ID not exist! Please check all the ID init correctlly!");
        }
    }
    public void GetControlFromHost(int id, bool isTurn){ //这个是接收的,每个主机从机都从这里获取信息,至于信息的串口需要补充.
        if(playerList.ContainsKey(id)){
            playerList[id].myTurn = isTurn; //这里Room的玩家列表和myself的玩家维护的是同一个对象,直接这里启用myself就可以进行操作了(如果是轮到自己的话)
        }
        else{
            Debug.LogError("ID not exist! Please check all the ID init correctlly!");
        }
    }

    public void SendSingleRoomInfo(){
        //发送就按照下面receive的逻辑发过去就可以了.
    }
    public void ReceiveSingleRoomInfoFromHost(float _currentPollution, int whichID, int[] newFactory, int[] newPosition, int[] Destoryposition){ //这个给从机用的
        //new factory用数字编号/或枚举来代替哪一个factoryType,可以直接在牌库里面调. position返回的是棋盘上的位置.
        //Destoryposition 是指 如果上一个玩家有拆除,拆除的位置是哪里.这里不做判断了,接收到什么就造什么拆什么.(但是自己造的会在自己的客户端里面做判断)
        currentPollution = _currentPollution;
        if(playerList.ContainsKey(whichID)){
            Player tempplayer = playerList[whichID] as Player;
            for(int i = 0; i<newFactory.Length;i++ ){
                Factory_Type newFactory_Type = myself.Deck[i]; //牌库里的第几个建筑 
                int index = newPosition[i];
                myChessBoard.BuildChess(index,newFactory_Type,tempplayer); //会自动在对应的棋盘上生成一个棋子,并且绑定player对象和位置以及类型.
            }
            for(int i = 0 ; i < Destoryposition.Length; i++){
                int index = Destoryposition[i];
                // myChessBoard.DestoryChess(index); 
                Chessbass tempchessbass = myChessBoard.chessBasses[index].GetComponent<Chessbass>();
                tempchessbass.owner = null ;
                // tempchessbass.myChess = null;
                //这个拆除的功能还没有做 其实只需要desotry gameObject(chess 而不是chessbass),然后把chessbass的owner重新设置为null就可以了.
            }
        }
        else{
            Debug.LogError("the ID is not exist in the current player list!");
        }
    }
    public void  ReceiveSingleRoomInfoFromGuest(int whichID, int[] newFactory, int[] newPosition, int[] Destoryposition){ //这个给主机用的
        if(playerList.ContainsKey(whichID)){
            Player tempplayer = playerList[whichID] as Player; //更新从机发送过来的内容.
            for(int i = 0; i < newFactory.Length; i++){
                Factory_Type newFactory_Type = myself.Deck[i]; //牌库里的第几个建筑 , 所有人的牌库的顺序都是一致的,所以用谁的deck都一样
                int index = newPosition[i];
                Chess newchess = myChessBoard.BuildChess(index,newFactory_Type,tempplayer).GetComponent<Chess>();
                tempplayer.ownChess.Add(newchess); // player 只关注chess.
            }
            for(int i = 0 ; i < Destoryposition.Length; i++){
                int index = Destoryposition[i];
                // tempplayer.ownChess.RemoveAt(i); // 我记得删除列表中的元素好像不可以这么写的否则顺序会错这里需要你修改一下.我不太记得了.. 之所以要删除是因为结算要把建造过的删除,从机则不需要管理那个列表因为他不参与计算
                // myChessBoard.DestoryChess(index);//删除对应的gameobject chess
                Chessbass tempchessbass = myChessBoard.chessBasses[index].GetComponent<Chessbass>(); //棋盘底座设置为null就可以了.如果它上面的chess被销毁这里myChess应该是失去引用了(不太确认,要测试一下是不是这样.)
                tempchessbass.owner = null ;
                // myChessBoard[index].myChess = null;

            }
        }
        else{
            Debug.LogError("the ID is not exist in the current player list!");
        }
    }
    // 游戏中
    // 游戏准备
    public void SendPlayerListinfo(){

    }
    public void ReceivedPlayerList(int[] allID, string[] allname){
        //这个功能只会调用一次,就是在主机init_player之后,发送所有玩家的信息给从机.后面小回合结算和生命值更新这些需要玩家列表.
            for(int i = 0; i < allID.Length; i++){
                Player newplayer = new Player();
                newplayer.ID = allID[i];
                newplayer.name = allname[i];
                // newplayer.ownChess = new List<Chess>(); //棋子列表需要初始化是因为从机的棋盘也需要读取信息.
                playerList.Add(newplayer.ID,newplayer);
                 //这时候从机也拥有了所有的类,但是其他类似EachRoundinfo这些信息是不知道的.只知道自己的.
                 //assignMyself才会初始化只属于自己的对象,别人的就不用管了.
            }
    }
    public void SendassignMyself(int whichid){
        //TODO 通讯.调用这个功能发送给某个客户端.
    }
    public void assignMyself(int id){ 
        //这个函数是由主机发送一个ID来告诉从机,当前玩家列表里面,哪一个玩家是“你自己”.
        //主机也可以调用这个.但是要注意要等playerList存在了“自己”并初始化了名字和唯一ID并在assign room info之后再调用这个.
        if(playerList.ContainsKey(id)){
            if(myself==null){
                Debug.LogError("myself not exist! Maybe should add mySelf class before assgin myself.");
                return;
            }
            myself.myself = playerList[id]; 
            if(roomInitInfo!=null){
                myself.myself.EachRoundInfo = new Dictionary<string,float>();
                myself.myself.EachRoundInfo.Add(constantString.current_own_gold,roomInitInfo.initGold); 
                myself.myself.EachRoundInfo.Add(constantString.grand_total_pollution,0);
                myself.myself.ownChess = new List<Chess>(); 
            }
            else{
                Debug.LogError("roomInitInfo not exist! Maybe should get the room info before assgin myself.");
            }
        }
        else{
            Debug.LogError("ID not exist! Maybe should get all the player info before assgin myself.");
        }
    }
    // 游戏准备


}
