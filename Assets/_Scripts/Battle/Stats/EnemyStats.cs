public class EnemyStats : CharacterStats
{
    protected async override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandleOppositTeamTurn();
        if (state == BattleState.EnemyTurn)
            await HandleYourTeamTurn();
    }

    protected override void HandleBodyColor()
    {
        if (GetComponent<CharacterSelection>().HasFinishedTurn && TurnManager.BattleState == BattleState.EnemyTurn)
            _bodySpriteRenderer.color = Helpers.GetColor("gray");
        else
            base.HandleBodyColor();
    }

}
