using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Items/Item With Status On Battle Start")]
public class ItemWithStatusOnBattleStart : Item
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
