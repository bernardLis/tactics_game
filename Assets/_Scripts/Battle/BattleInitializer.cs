using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{

    void Start()
    {
        Hero playerHero = GameManager.Instance.PlayerHero;
        List<ArmyGroup> playerArmy = playerHero.Army;
        Hero opponentHero = GameManager.Instance.SelectedBattle.Opponent;
        List<ArmyGroup> opponentArmy = opponentHero.Army;

        BattleManager.Instance.Initialize(playerHero, opponentHero, playerArmy, opponentArmy);
    }

}
