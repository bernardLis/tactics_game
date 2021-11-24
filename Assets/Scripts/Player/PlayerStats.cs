using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    protected override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state != BattleState.PlayerTurn)
            return;

        base.TurnManager_OnBattleStateChanged(state);
    }
}
