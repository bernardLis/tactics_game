public class PlayerStats : CharacterStats
{
    protected override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            base.TurnManager_OnBattleStateChanged(state);
        if(state == BattleState.EnemyTurn)
            ResolveModifiersTurnEnd();
    }
}
