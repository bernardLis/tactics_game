public class PlayerStats : CharacterStats
{
    protected async override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            await ResolveStatuses();
        if (state == BattleState.EnemyTurn)
            ResolveModifiersTurnEnd();
    }

    protected override void HandleBodyColor()
    {
        if (GetComponent<CharacterSelection>().HasFinishedTurn && TurnManager.BattleState == BattleState.PlayerTurn)
            _bodySpriteRenderer.color = Helpers.GetColor("gray");
        else
            base.HandleBodyColor();
    }

}
