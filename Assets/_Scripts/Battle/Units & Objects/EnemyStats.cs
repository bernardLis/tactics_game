public class EnemyStats : CharacterStats
{
    protected override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            ResolveModifiersTurnEnd();

        if (state == BattleState.EnemyTurn)
            base.TurnManager_OnBattleStateChanged(state);
    }

    
}
