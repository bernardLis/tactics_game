public class EnemyStats : CharacterStats
{
    public bool TurnStartResolved;
    protected override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            ResolveModifiersTurnEnd();

        // HERE: I would like to wait for this to resolve before I select 
        if (state == BattleState.EnemyTurn)
        {
            base.TurnManager_OnBattleStateChanged(state);
            // HERE: this is wrong, coz it won't wait for statuses to resolve...
            TurnStartResolved = true;
        }
    }


}
