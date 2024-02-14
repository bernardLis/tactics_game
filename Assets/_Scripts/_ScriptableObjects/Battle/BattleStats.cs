using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleStats : ScriptableObject
{
    public int MinionsKilled;
    public int CreaturesKilled;

    public int TilesUnlocked;

    public int VasesBroken;
    public int CoinsCollected;
    public int HammersCollected;
    public int HorseshoesCollected;
    public int BagsCollected;
    public int SkullsCollected;
    public int FriendBallsCollected;

    public int ExpOrbsCollected;

    public int FriendBallsThrown;


    public void Initialize()
    {
        MinionsKilled = 0;
        CreaturesKilled = 0;
        TilesUnlocked = 0;
        VasesBroken = 0;
        CoinsCollected = 0;
        HammersCollected = 0;
        HorseshoesCollected = 0;
        BagsCollected = 0;
        SkullsCollected = 0;
        FriendBallsCollected = 0;
        ExpOrbsCollected = 0;
        FriendBallsThrown = 0;
    }
}