using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInformation", menuName = "Game/GameInformation", order = 1)]
public class GameInitInfo : ScriptableObject
{
    public float TotalPollution = 100;
    public int TotalRound = 15;
    public float EachRoundTime = 50; // secs
    public float InitGold = 100;
    public List<FactoryType> FactoryTypes; //这个部分assing给玩家,因为不需要发牌了
    public int FirstId = 0;
}
