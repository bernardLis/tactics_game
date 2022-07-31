using UnityEngine;
using System.Threading.Tasks;
public class EnemyCharSelection : CharacterSelection
{
    protected override void HandleEnemyTurn()
    {
        if (!_myStats.IsStunned)
        {
            SetHasFinishedTurn(false);
            SetCharacterColor(Color.white);
        }
        else
            FinishCharacterTurn();
    }

    protected override async void HandlePlayerTurn()
    {
        if (_myStats.IsStunned)
            return;

        await Task.Delay(1000);
        SetCharacterColor(Color.white);
    }
    
    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        _turnManager.EnemyCharacterTurnFinished(gameObject);
    }

}
