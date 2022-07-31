using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;

public class PlayerCharSelection : CharacterSelection
{
    public bool HasMovedThisTurn { get; private set; }
    public Vector3 PositionTurnStart; //{ get; private set; }


    public override void Awake()
    {
        base.Awake();
    }

    public bool CanBeSelected()
    {
        return !HasFinishedTurn;
    }

    protected override void HandlePlayerTurn()
    {
        if (!_myStats.IsStunned)
        {
            HasMovedThisTurn = false;
            SetHasFinishedTurn(false);
        }
        else
            FinishCharacterTurn();

        // remember on which tile you start the turn on 
        PositionTurnStart = transform.position;
    }

    protected override async void HandleEnemyTurn()
    {
        if (_myStats.IsStunned)
            return;
        
        await Task.Delay(1000);
        SetCharacterColor(Color.white);
    }

    public void SelectCharacter()
    {
        GetComponent<SortingGroup>().sortingOrder = 99;
        _myStats.Select();
        ToggleSelectionArrow(true);
    }

    public void DeselectCharacter()
    {
        GetComponent<SortingGroup>().sortingOrder = 90;

        // hide the arrow
        ToggleSelectionArrow(false);
    }


    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        DeselectCharacter();

        // finish character's turn after the interaction is performed
        _turnManager.PlayerCharacterTurnFinished(gameObject);
    }

    public void SetCharacterMoved(bool hasMoved)
    {
        HasMovedThisTurn = hasMoved;
    }

    public void SetPositionTurnStart(Vector3 pos)
    {
        PositionTurnStart = pos;
    }
}

