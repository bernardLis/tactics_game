using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    protected override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state != BattleState.EnemyTurn)
            return;

        base.TurnManager_OnBattleStateChanged(state);
    }
}
