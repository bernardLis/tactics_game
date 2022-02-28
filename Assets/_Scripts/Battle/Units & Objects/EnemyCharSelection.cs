public class EnemyCharSelection : CharacterSelection
{
    protected override void HandleEnemyTurn()
    {
        if (!_myStats.IsStunned)
        {
            SetHasFinishedTurn(false);
            ReturnCharacterColor();
        }
        else
            FinishCharacterTurn();
    }

    protected override void HandlePlayerTurn()
    {
        if (!_myStats.IsStunned)
            Invoke("ReturnCharacterColor", 1f);
    }
    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        _turnManager.EnemyCharacterTurnFinished();
    }

}
