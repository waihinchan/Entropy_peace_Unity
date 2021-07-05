using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Myself: MonoBehaviour
{   
    float RandomEndMyTurnParams = 0.5f;
    public Room myHost{get;set;} 
    public void checkMyturn(){
        if(myself!=null){
            if(myself.myTurn!=myTurn){
                myTurn = myself.myTurn;
                if(myTurn){
                    Debug.Log($"i am {myself.name} now is my fucking turn !!!!!!!!!!!!!! {myTurn}");
                    this.StartCoroutine(FireAfterOneSec(Random.Range(1.0f,3.0f))); //After random sec random fire if want to end my turn
                }
                else{
                    Debug.Log($"i am {myself.name} now is not my fucking turn !!!!!!!!!!!!!! {myTurn}");
                }
            }
        }
    }
    public IEnumerator FireAfterOneSec(float _randomAfter){
        yield return new WaitForSeconds(_randomAfter);
        RandomEndMyTurn();
        yield break;
    }
    public void RandomEndMyTurn(){
        if(myTurn){//只有在自己回合下才可以结束自己的回合
            if(Random.Range(0,1)<RandomEndMyTurnParams){
                myHost.stopMyTrun();
                Debug.Log($"i am {myself.name}, i just end my turn in advanced!");
            }
        }
    }
    public void updateMyOwnChess(Chess _newchess){
        int record = myself.ownChess.Count;
        
        myself.ownChess.Add(_newchess);
        Debug.Log(record - myself.ownChess.Count);
        // Debug.Log($"i am {myself.name}, my id is {myself.ID} i build a {_newchess.myFactory_Type.name}");
    }
    public void randomBuild(){ //只有在自己的回合下才可以随机建造
       
        if(myTurn){
            int randomPick = (int)Mathf.Floor(Random.Range(0,Deck.Count)); //随机抽一个建筑来建造
            float targetGold = Deck[randomPick].cost_gold; //获得建造这个建筑所花费的金钱
            int randomSlot = (int)Mathf.Floor(Random.Range(0,myHost.myChessBoard.totalChessBass));//随机找一个位置来建造
            if(myHost.SlotEmpty(randomSlot)){ 
                //先判断那个位置是否已经有棋子占据了.后面做拖放的时候,直接用raycaster来判定,在棋盘里面找索引有点麻烦
                if(judgeBuild(targetGold)){
                    //判断是否扣钱,因为如果先做的话钱已经扣了但是可能建筑没有摆放上去.
                    Chess newchess = myHost.myChessBoard.BuildChess(randomSlot,Deck[randomPick],this.myself).GetComponent<Chess>();
                    
                    updateMyOwnChess(newchess);
                    // myHost.updateInfo(myself);
                    
                    //这里只是用于测试..应该是由Room对象发送信息给主机,
                    //把自己的信息传递给myhost,(myhost不直接获取游戏中其他“从机”里面的myself对象,因为维护房间状态只需要维护playerlist)
                }   
            }

            
        }
    }
}
public partial class Room : MonoBehaviour
{   
    //这里只适用于单机模式下
    //单机模式下会自动生成一些假玩家,并且让玩家随机操作
    List<GameObject> debugObjects = new List<GameObject>(); //这里充当串口.
    public void updateInfo(Player updateTarget){
        //InvalidOperationException: Collection was modified; enumeration operation may not exe·te.
        //所以不能直接传数值
        if(roomInitInfo.isHost){
            if(playerList.ContainsKey(updateTarget.ID)){
                playerList[updateTarget.ID].ownChess = updateTarget.ownChess;
                Debug.Log($"player {updateTarget.name} with ID {updateTarget.ID},  build a newchess.myFactory_Type.name");
            }
        }
    }
    void DebugMode(){
        // Debug.Log("now running debugmode");
        if(roomInitInfo.Debug){
            if(myself!=null){
                myself.randomBuild();
                myself.checkMyturn(); //这个只是用于检查小回合轮转的时候是否正确.
            }
            if(roomInitInfo.isHost){//只有host才会生成东西
                if(!gameStart){
                    RoomInitInfo temproominitinfo = new RoomInitInfo();
                    temproominitinfo.totalPollution = roomInitInfo.totalPollution;
                    temproominitinfo.totalRound = roomInitInfo.totalRound;
                    temproominitinfo.eachRoundTime = roomInitInfo.eachRoundTime;
                    temproominitinfo.initGold = roomInitInfo.initGold;
                    temproominitinfo.factory_types = roomInitInfo.factory_types;
                    temproominitinfo.totalPlayer = roomInitInfo.totalPlayer;
                    temproominitinfo.Debug = roomInitInfo.Debug;
                    temproominitinfo.isHost = false;
                    for(int i = 0; i < roomInitInfo.totalPlayer-1;i++){ 
                        //塞满,除了自己
                        int randomID = (int)Random.Range(0,10000); //先随机一个id
                        while(playerList.ContainsKey(randomID)){ //基本上不会有重复的..
                            Debug.Log("find same");
                            randomID = (int)Random.Range(0,10000);
                        }
                        Player fakePlayer = new Player();
                        playerList.Add(randomID,fakePlayer);
                        GameObject fakeContainer = new GameObject(); //这里视作一个假玩家的客户端
                        fakeContainer.name = i.ToString();
                        Room fakeroom = fakeContainer.AddComponent<Room>();

                        fakeroom.assignRoominfo(temproominitinfo); //重新指派roominfo (这里还会重新指派一个myself)
                        fakeroom.myself.myHost = this; //这里做一个假通讯,把所有gameobject(包括自己)的Host指派给主机.
                        debugObjects.Add(fakeContainer);
                        Debug.Log("add a player");

                    }
                    { //最后把自己加上去
                    int randomID = (int)Random.Range(0,10000); 
                    while(playerList.ContainsKey(randomID)){ 
                        randomID = (int)Random.Range(0,10000);
                    }
                    Player fakePlayer = new Player();
                    playerList.Add(randomID,fakePlayer);
                    debugObjects.Add(gameObject); 
                    myself.myHost = this;
                    Debug.Log("add a host at last");
                    }
                    //此时debugOb·ects和player还不是一一对应的,但是主机是排在列表的最后.

                }
            }
        }
        else{
            Debug.LogError("Mode not run correctlly");
        }
    }

    void sendplayertomyself(){
        if(debugObjects.Count!=playerList.Count){
            Debug.LogError("Debug object and Playerlist not match!");
        }
        
        IDictionaryEnumerator tempEnumerator = playerList.GetEnumerator();
        tempEnumerator.MoveNext();
        foreach (GameObject debugObject in debugObjects)
        {
            debugObject.GetComponent<Myself>().myself = tempEnumerator.Value as Player;
            int id = (int)tempEnumerator.Key;
            debugObject.GetComponent<Myself>().myself.ID = id;
            tempEnumerator.MoveNext();
            
        }
    }
}
