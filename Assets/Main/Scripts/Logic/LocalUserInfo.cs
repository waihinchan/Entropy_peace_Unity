using UnityEngine;

[CreateAssetMenu(fileName = "LocalUserInfo", menuName = "User/LocalUserInfo", order = 1)]
public class LocalUserInfo: ScriptableObject
{
    public int GameMoney = 100;
    public string UserName = "unknown";
    public int HistoryWin;
    public int HistoryFail;
    public int HistoryTie;
}