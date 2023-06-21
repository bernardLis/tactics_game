using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    void Start()
    {
        Hero playerHero = GameManager.Instance.PlayerHero;
        List<Creature> playerArmy = playerHero.Army;
        Hero opponentHero = GameManager.Instance.SelectedBattle.Opponent;
        List<Creature> opponentArmy = opponentHero.Army;

        BattleManager.Instance.Initialize(playerHero, opponentHero, playerArmy, opponentArmy);
    }
}
