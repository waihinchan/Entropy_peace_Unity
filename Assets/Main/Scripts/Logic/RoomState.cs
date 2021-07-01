using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum RoomStateenum{
        WaitingForStart, //这个等待开始可以和网络通信的等待合并成同一个.如果有必要做的话
        StartGame,
        InTruns, //这个是玩家轮换
        TurnsEnd, //这个是所有玩家轮换完一次计算为一个回合
        End //游戏结束

    }

    public abstract class RoomState 
    {
        protected Room runningRoom;

        public RoomState(Room _runningRoom){ //构造器,把房间状态传递给这个类.
            runningRoom = _runningRoom;
        }
        public virtual IEnumerator StartState(){
            yield break;
        }
        public virtual IEnumerator UpdateState(){
            yield break;
        }
        public virtual IEnumerator EndState(){
            yield break;
        }
        public void close(){}//if we could write a dispose here?
    }
    public class WaitingForStart:RoomState{
        bool waiting = false;
        public WaitingForStart(Room _runningRoom):base(_runningRoom){
            //其他需要构造的东西.或者调用Room的GUI之类的东西
        }
        public override IEnumerator StartState(){
            waiting = true;
            runningRoom.StartCoroutine(this.UpdateState());//start to waiting for people
            yield break;
        }
        public override IEnumerator UpdateState(){    
            while(waiting){ //这里需要写一下结束等待的条件,例如满人了或者玩家退出房间之类的.
                Debug.Log("waiting for people");
                yield return null;
            }
            runningRoom.StartCoroutine(this.EndState()); //移动到下一个状态.
            yield break;
        }
        public virtual IEnumerator EndState(){
            if(runningRoom.roomInitInfo!=null){ //awake的时候init了房间信息,这里是等待满人初始化所有玩家的信息(本场牌局能用的建筑、初始金钱等等)
                runningRoom.init_player(); 
            }
            runningRoom.CurrentState = new StartGame(runningRoom,runningRoom.StartCountDown); 
            //结束状态,除了做其他处理以外,进入到下一个新的状态.等待玩家的下一个状态是开始游戏/或者是直接这个房间就消失了之类的.
            //然后把房间的当前状态转移到下一个状态,然后让房间的当前状态启用它自己的维护.
            runningRoom.StartCoroutine(runningRoom.CurrentState.StartState()); 
            yield break;
        }
    }
    public class StartGame:RoomState{
        int startCountDown;
        public StartGame(Room _runningRoom,int _startCountDown = 10):base(_runningRoom){
            startCountDown = _startCountDown;
        }
        public override IEnumerator StartState(){
            while(startCountDown>0){
                Debug.Log($"Game Start After {startCountDown} sec");
                startCountDown--;
                yield return new WaitForSeconds(1);
            }
            runningRoom.StartCoroutine(this.UpdateState());//start to waiting for people
            yield break;
        }
        public override IEnumerator UpdateState(){
            //这里写开始游戏后的一些更新准备状态.也可以把倒计时搬来这里
            runningRoom.StartCoroutine(this.EndState());
            yield break;
        }
        public override IEnumerator EndState(){
            runningRoom.CurrentState = new InTruns(runningRoom,runningRoom.turnCountDown); //开始游戏后就是开始轮换玩家和回合结束的状态机.
            runningRoom.StartCoroutine(runningRoom.CurrentState.StartState());//开始维护轮换玩家的状态机 
            yield break;
        }
    }
    public class InTruns:RoomState{
        //轮换的判断是按顺序来排列玩家.后面也有可能会有打乱顺序之类的功能,所以这个是需要直接调用房间里面的对象,这里不要做任何运算.
        //此外轮换的时候是会发生一些状态判断的,比如说当前生命值归0的情况.此时是不结束回合直接结束游戏/或者进入到结束回合然后结束游戏.
        float turnCountDown;
        float currentCountDown;

        IDictionaryEnumerator CurrentPlayer; //这个每一次新建都会初始化,也就是从第一个重新开始.
        public InTruns(Room _runningRoom,float _turnCountDown = 45.0f):base(_runningRoom){
            CurrentPlayer = _runningRoom.playerList.GetEnumerator(); //在这一轮就开始获取指针.
            if(_turnCountDown<=0){
                turnCountDown = 45.0f;
            }
            else{
                turnCountDown = _turnCountDown;
            }
        }
        public override IEnumerator StartState(){
            Debug.Log($"Start turns {runningRoom.CurrentTurn} in / {runningRoom.TotalTurns}");
            //这里应该要屏蔽掉其他当前不在这个回合的玩家的之类的.
            runningRoom.StartCoroutine(this.UpdateState());
            yield break;
        }
        public override IEnumerator UpdateState(){
            ResetState(); //先重置所有的计数器.
            runningRoom.StartCoroutine(this.EachPlayerCountDown()); //实际上的update维护是由下面的这个来做的
            yield break;
        }
        IEnumerator EachPlayerCountDown(){
            Player currentPlayer = CurrentPlayer.Value as Player;

            Debug.Log($"Current Player is {currentPlayer.name}"); //当前指针指向的数值 -> 玩家 -> 姓名.
            while(CheckState()){
                yield return new WaitForSeconds(1);
                if(runningRoom.game_end){ //每秒检测一次,除了单个玩家的倒计时以外还要判断游戏是否结束了,如果结束了直接终止这个协程并转移到下一个状态阶段
                    runningRoom.StartCoroutine(this.EndState());
                    yield break;
                }
                currentCountDown--;
                // senddatatocurrentplayer(); //每一秒的倒计时都发送信息给玩家告诉他当前的剩余时间.
            }
            if(currentCountDown<=0){
                runningRoom.Smallsettle(); //在移动到下一个之前,结算这个玩家的信息.(小回合结算)
            }
            if(CurrentPlayer.MoveNext()){ //上面回合倒计时结束之后就移动到下一个玩家
                ResetState(); //重置计数器
                IEnumerator nextcoroutine = this.EachPlayerCountDown(); 
                runningRoom.StartCoroutine(nextcoroutine);
            }
            else{ //到最后一个玩家,意味着这个回合结束了
                runningRoom.StartCoroutine(this.EndState());
            }
            yield break; //结束意味着单个玩家的小回合结束了,大回合还要再循环
        }
        bool CheckState(){
            return (currentCountDown>0 && !runningRoom.game_end && !runningRoom.turn_end);
        }
        void ResetState(){
            runningRoom.turn_end = false; //重置玩家回合是否结束的标志
            currentCountDown = turnCountDown; //重置计数器
        }
        public override IEnumerator EndState(){
            Debug.Log($"All player end their turns!");
            runningRoom.CurrentState = new TurnsEnd(runningRoom); //这个pass到回合结算阶段.回合结算阶段用于判断是否进入下一个回合还是直接结束游戏(如果说时间到了或者生命值归0)
            runningRoom.StartCoroutine(runningRoom.CurrentState.StartState());//开始维护轮换玩家的状态机 
            yield break;
        }
    }

    public class TurnsEnd:RoomState{
        public TurnsEnd(Room _runningRoom):base(_runningRoom){

        }
        public override IEnumerator StartState(){
            runningRoom.Bigsetttle(); //大回合结算,开始状态就是结算所有玩家.然后结算内会调用判断生命值的情况.如果生命值归0就游戏结束了.
            if(!runningRoom.game_end){ //判断是否游戏结束(这里的游戏结束是由房间内因为生命值归0实时更新的游戏结束状态)
                if(runningRoom.CurrentTurn<=0){ //如果回合数走到最后一个回合,也代表回合结束.回合是由状态机来维护的.
                    runningRoom.game_end = true;
                }
            }
            runningRoom.StartCoroutine(this.EndState());
            //把游戏结束状态到信息传递给endstate来判断是否开启下一个大回合还是结束游戏状态.
            yield break;
        }

        public override IEnumerator EndState(){
            if(!runningRoom.game_end){
                Debug.Log($"Turns end in {runningRoom.CurrentTurn} / {runningRoom.TotalTurns}");
                float nextTrunStartCountDown = 5;
                Debug.Log($"Turns end in {runningRoom.CurrentTurn + 1} start in nextTrunStartCountDown secs");
                while(nextTrunStartCountDown>0){ //下一个回合开始的倒计时.
                    yield return new WaitForSeconds(1);
                    nextTrunStartCountDown-=1;
                    Debug.Log($"Turns end in {runningRoom.CurrentTurn + 1} start in nextTrunStartCountDown secs");
                    //senddatatoallplayer(); //发送信息给所有玩家告诉他们剩余的时间
                    yield break;
                }
                runningRoom.CurrentTurn--; //回合数-1
                runningRoom.CurrentState = new InTruns(runningRoom,runningRoom.turnCountDown); //继续到下一个大回合
                runningRoom.StartCoroutine(runningRoom.CurrentState.StartState());//开始维护轮换玩家的状态机
                //这里还需要调用一些updateGUI信息之类到内容
                yield break;//结束这个state
            }
            else{
                //这里还需要调用一些updateGUI信息之类到内容
                runningRoom.CurrentState = new End(runningRoom); //进入结束游戏状态机
                runningRoom.StartCoroutine(runningRoom.CurrentState.StartState());
                yield break;
            }
            yield break;
        }
    }
    public class End:RoomState{
        public End(Room _runningRoom):base(_runningRoom){
            //其他需要构造的东西.或者调用Room的GUI之类的东西
        }
    }
 