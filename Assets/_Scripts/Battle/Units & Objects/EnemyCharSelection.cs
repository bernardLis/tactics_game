public class EnemyCharSelection : CharacterSelection
{
    protected override void HandleEnemyTurn()
    {
        if (!myStats.isStunned)
        {
            SetHasFinishedTurn(false);
            ReturnCharacterColor();
        }
        else
            FinishCharacterTurn();
    }

    protected override void HandlePlayerTurn()
    {
        if (!myStats.isStunned)
            Invoke("ReturnCharacterColor", 1f);
    }
    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        turnManager.EnemyCharacterTurnFinished();
    }

}
