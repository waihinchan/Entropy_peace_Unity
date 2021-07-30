using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameContext
{
    public bool MyDone;
    public bool OpDone;
    public bool IsMyTurn;
    // 当前回合经历时间
    public float TurnTime;
    // 回合数
    public int TurnCount;
    public int Pollution;
    public GameContext(bool isMyTurn)
    {
        IsMyTurn = isMyTurn;
        OpDone = false;
        MyDone = false;
        TurnTime = 0f;
        
        TurnCount = 1;
        Pollution = 0;
    }
}

public class Game : MonoBehaviour
{
    public GameContext GameContext;
    private UserManager _userManager;
    private GameInfo _gameInfo;
    public void Start()
    {
        _userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        _gameInfo = _userManager.GameInfo;
        GameContext = new GameContext(_gameInfo.Id == _gameInfo.FirstId);
    }

    public void PlayAgain()
    {
        _userManager.JoinRoom(_userManager.Host, _userManager.Username); ;
    }

    public void Return(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}