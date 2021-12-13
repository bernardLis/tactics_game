public class EnemyCharSelection : CharacterSelection
{
    protected override void HandleEnemyTurn()
    {
        // reseting flags on turn's end
        hasFinishedTurn = false;
    }

    protected override void HandlePlayerTurn()
    {
        if (!myStats.isStunned)
            Invoke("ReturnCharacterColor", 1f);
    }
    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        highlighter.ClearHighlightedTiles().GetAwaiter();
        TurnManager.instance.EnemyCharacterTurnFinished();
    }

}
