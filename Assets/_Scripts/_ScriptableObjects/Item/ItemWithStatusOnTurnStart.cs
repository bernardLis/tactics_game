using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Items/Item With Status On Turn Start")]
public class ItemWithStatusOnTurnStart : Item
{

    CharacterStats _stats;
    public override void Initialize(CharacterStats stats)
    {
        base.Initialize(stats);
        _stats = stats;

        TurnManager.OnBattleStateChanged += OnBattleStateChanged;
    }

    void OnBattleStateChanged(BattleState state)
    {
        if (_stats.CompareTag(Tags.Player) && state == BattleState.PlayerTurn)
            AddStatus();
        if (_stats.CompareTag(Tags.Enemy) && state == BattleState.EnemyTurn)
            AddStatus();
    }

    async void AddStatus() { await _stats.AddStatus(Status, null); }
}
