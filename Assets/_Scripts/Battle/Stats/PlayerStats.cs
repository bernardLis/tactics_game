public class PlayerStats : CharacterStats
{
    protected async override void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            await HandleYourTeamTurn();
        if (state == BattleState.EnemyTurn)
            HandleOppositTeamTurn();
    }

    protected override void HandleBodyColor()
    {
        if (GetComponent<CharacterSelection>().HasFinishedTurn && TurnManager.BattleState == BattleState.PlayerTurn)
            _bodySpriteRenderer.color = Helpers.GetColor("gray");
        else
            base.HandleBodyColor();
    }

}
