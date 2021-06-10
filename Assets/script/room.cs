using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// room 的主要作用是:
// 计算数值,分配回合,随机发牌,返回数值给player用于GUI的显示(总生命值什么的,其实也可以在一个类下面做)
// 房间的基本上写完了.
//接下来是player的交互请求 和一些GUI的显示之类的
// 流程:
// 接收一个asset
// asset包含了 回合数 生命值 每回合倒计时 和牌库
// 玩家因为目前默认就2个所以直接初始化2个玩家,一个是AI的一个是普通玩家
// 游戏开始开始一个协程(这里可能还要一个倒计时条件后再开始也可以)
// 然后协程会运行直到游戏结束.
// 每间隔固定时间发牌、更新回合
// 因为是协程所以理论上玩家请求roll也不影响,而其他的数值
public class room : MonoBehaviour
{   
    public SceneInitClass sceneinit;
    private int rounds;
    private List<GameObject> player_list; //这里应该要用一个get?
    private float HP;
    private float totalHP;
    private List<GameObject> chess_list;
    private bool game_end = false;
    private float countdown;
    private float totaltime;
    private List<Factory_type> all_factories;
    private IEnumerator eachround;
    private IEnumerator counttime;
    // Start is called before the first frame update
    //GUI 这个部分应该是放在playerscript并且做成prefab加载出来 但是因为我们是单人所以直接由room来给
    public Text Time_left;
    public Text R;
    public Text E;
    public Text P;
    public Text R2;
    public Text HP_text;
    public Image HP_image;
    public Button Roll;
    public Button slot1;
    public Button slot2;
    public Image Imageslot1;
    public Image Imageslot2;
    public GameObject endimage;
    public Vector3 offsetofchess;
    void Awake(){
        // print("Starting " + Time.time);
        // Start function WaitAndPrint as a coroutine.
        // print("Before WaitAndPrint Finishes " + Time.time);
        // sceneinit = GameObject.Find("settingpass").GetComponent<passsetting>().sceneinit;
        if(sceneinit != null){
            init_room(sceneinit);
        }
        else{
            //  Debug.Log("give me a fking asset ");
        }
        eachround = startroom(countdown);
        StartCoroutine(eachround);
        counttime = showtimeleft();
        StartCoroutine(counttime);

    }
    void Start()
    {   


    }

    // Update is called once per frame
    void Update()
    {      
        update_GUI(player_list[0]);
        // Debug.Log(player_list[1].GetComponent<playerscript>().is_AI);
        //基本上类似请求建造这些都是在player内操作的
        //所以好像类似rollback或者是申请roll这种也不影响协程
    }
    void init_room(SceneInitClass sceneinit){
        // i am a fxxking genius
        // place_chess(sceneinit);
        init_player(sceneinit);
        all_factories = new List<Factory_type>();
        foreach (Factory_type single_factory in sceneinit.factory_types)
        {   
            for(int i = 0; i < (single_factory.maxnum*player_list.Count);i++){
                all_factories.Add(single_factory);

            }
            
        }
        HP = sceneinit.HP; 
        totalHP = sceneinit.HP;
        rounds = sceneinit.rounds;
        countdown = sceneinit.countdown;//how many sec in 1 round
        totaltime = countdown;
        free_roll();
        update_GUI(player_list[0]);

        
    }

    void init_player(SceneInitClass sceneinit){
        playerscript.init_power = sceneinit.init_power;
        playerscript.inmit_resource = sceneinit.inmit_resource;
        player_list = new List<GameObject>(); //player for recording the rank
        place_chess(sceneinit);
        GameObject player = new GameObject();
        GameObject AI = new GameObject();
        player.name = sceneinit.player_name;
        AI.name = "AI";
        player.AddComponent<playerscript>();
        AI.AddComponent<playerscript>();
        AI.AddComponent<AIlogic>();
        player.GetComponent<playerscript>().is_AI = false;
        AI.GetComponent<playerscript>().is_AI = true;
        AI.GetComponent<AIlogic>().rollcost = sceneinit.rollcost;
        player_list.Add(player);
        player_list.Add(AI);
        Roll.GetComponent<Roll>().Player = player;
        Roll.GetComponent<Roll>().rollcost = sceneinit.rollcost;
        build.current_player = player;
    }

    void place_chess(SceneInitClass sceneinit){
        chess_list = new List<GameObject>(); // we need all the value in this list
        Renderer rend;
        GameObject chess_board = sceneinit.chess_unit.transform.GetChild(1).gameObject;
        rend = chess_board.GetComponent<Renderer>();
        Vector3 unit_size = rend.bounds.size;
        int row = (int)Mathf.Sqrt(sceneinit.checkerboards);
        int col = row;
        Vector3 unit_position;
        for(int i = 0; i < row; i++){
            for(int j = 0; j < col; j++){
                Vector3 current_index = new Vector3((float)i*sceneinit.interval,0,(float)j*sceneinit.interval);
                current_index += offsetofchess;
                unit_position = Vector3.Scale(unit_size , current_index); 
                GameObject chess = Instantiate(sceneinit.chess_unit);
                chess.transform.position = unit_position;
                chess_list.Add(chess);
            }      
        }
        Debug.Log(chess_list.Count);
    }

    public Factory_type roll(Factory_type rollback_factory){ //不太确定能不能这么写.....
        if(rollback_factory==null){ //if the slot is empty, mean assing a factory directly
            return assign_factory(); 
        }
        else{
            rollback(rollback_factory); //otherwise rollback the slot and reroll
            return assign_factory();
        }
    }

    void rollback(Factory_type rollback_factory){//这个应该在每次roll的时候调用
        all_factories.Add(rollback_factory);
    }
    
    Factory_type assign_factory(){ //这个应该是可以用来比如说执行两次就是获取两个, rollback需要外部调用或者是怎么样访问咯
        int randomindex = Random.Range(0,all_factories.Count);
        Factory_type assign = all_factories[randomindex];
        all_factories.Remove(assign);
        // Debug.Log("assgin a " + assign);
        return assign;
    }

    void free_roll(){
        //not sure this is working
        foreach (GameObject singleplayer in player_list) 
        {   
            singleplayer.GetComponent<playerscript>().Slot1 = roll(singleplayer.GetComponent<playerscript>().Slot1);
            singleplayer.GetComponent<playerscript>().Slot2 = roll(singleplayer.GetComponent<playerscript>().Slot2);
        }
        updatebutton();
    }   
    public void updatebutton(){ //this is disaster
        slot1.GetComponent<build>().Current_slot = player_list[0].GetComponent<playerscript>().Slot1;
        slot2.GetComponent<build>().Current_slot = player_list[0].GetComponent<playerscript>().Slot2;
        Imageslot1.sprite = slot1.GetComponent<build>().Current_slot.mySprite;
        Imageslot2.sprite = slot2.GetComponent<build>().Current_slot.mySprite;
        Imageslot1.enabled = true;
        Imageslot2.enabled = true;

        // slot1.transform.GetChild(0).gameObject.GetComponent<Text>().text = player_list[0].GetComponent<playerscript>().Slot1.name;
        // slot2.transform.GetChild(0).gameObject.GetComponent<Text>().text = player_list[0].GetComponent<playerscript>().Slot2.name;

    }
    void update_room(){ //这里可以优化一下如直接iter玩家而非建筑本身
        print("update room");
        totaltime = countdown;
        foreach (GameObject singleplayer in player_list)//
        {
            HP-= singleplayer.GetComponent<playerscript>().settle(); //Caculate the pollution. the player will get output in player scripts
        }
        rounds-=1;
        if(HP<=0||rounds==0){
            game_end = true;
            //write something here.
        }
        if(game_end){
            endimage.SetActive(true);
            if(HP<=0){
                endimage.transform.GetChild(0).gameObject.SetActive(true);
            }
            else{
                if(player_list[0].GetComponent<playerscript>().population>=player_list[1].GetComponent<playerscript>().population){
                    endimage.transform.GetChild(1).gameObject.SetActive(true);
                }
                else{
                    endimage.transform.GetChild(2).gameObject.SetActive(true);
                }
            }
            Debug.Log("my" + player_list[0].GetComponent<playerscript>().population.ToString());
            Debug.Log("AI" + player_list[1].GetComponent<playerscript>().population.ToString());
        }
        // update_GUI(player_list[0]);
    }
    void update_player_msg(GameObject player){
        R.text = "Resource: " + player.GetComponent<playerscript>().resource.ToString();
        E.text = "Electricity: " + player.GetComponent<playerscript>().electricity.ToString();
        P.text = "Population: " + player.GetComponent<playerscript>().population.ToString();
        
    }
    void update_GUI(GameObject player){
        Time_left.text = "Time: " + totaltime.ToString() + "s";
        update_player_msg(player);
        R2.text = "Round: " + rounds.ToString() + "/" + sceneinit.rounds.ToString();
        HP_text.text = "Global warming gases upper limit: " + HP.ToString();
        HP_image.fillAmount = HP/totalHP;
        if(slot1.GetComponent<build>().Current_slot==null){
            Imageslot1.enabled = false;
        }
        if(slot2.GetComponent<build>().Current_slot==null){
            Imageslot2.enabled = false;
        }
    }
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
    private IEnumerator showtimeleft(){
        while (!game_end)//当游戏还没有结束当时候
        {
            yield return new WaitForSeconds(1);
            totaltime--;
        }
    }
    public void AIbuild(Factory_type thatfactory){
        foreach (GameObject singlechess in chess_list)
        {
            if(singlechess.GetComponent<chess>().Owner==null){
                player_list[1].GetComponent<playerscript>().resource -= thatfactory.cost_resource;
                player_list[1].GetComponent<playerscript>().electricity -= thatfactory.cost_energy;
                singlechess.GetComponent<chess>().Owner = player_list[1];
                singlechess.GetComponent<chess>().get_build(thatfactory,player_list[1]);
                player_list[1].GetComponent<playerscript>().Owning_chesses.Add(singlechess);

                break;
            }
            else{
                continue;
            }
        }
    }
    public void backtostart(){
        SceneManager.LoadScene("setting");
    }
    


}
//每个回合都是倒计时即可
//只有更新数值是按照回合计