using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingOfSafety : Item
{

    CharacterStats _stats;
    public override void Initialize(CharacterStats stats)
    {
        base.Initialize(stats);
        _stats = stats;

        TurnManager.OnBattleStateChanged += OnBattleStateChanged;
    }

    async void OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.CurrentTurn == 1 && state == BattleState.PlayerTurn)
            await _stats.AddStatus(Status, _stats.gameObject);
    }
}
