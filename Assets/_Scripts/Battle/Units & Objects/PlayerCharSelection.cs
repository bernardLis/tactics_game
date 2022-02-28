using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCharSelection : CharacterSelection
{
    public bool HasMovedThisTurn { get; private set; }
    public Vector3 PositionTurnStart { get; private set; }

    [SerializeField] SelectionArrow _selectionArrow;

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

    protected override void HandleEnemyTurn()
    {
        if (!_myStats.IsStunned)
            Invoke("ReturnCharacterColor", 1f);
    }

    public void SelectCharacter()
    {
        GetComponent<SortingGroup>().sortingOrder = 99;
        ToggleSelectionArrow(true);
    }

    public void DeselectCharacter()
    {
        GetComponent<SortingGroup>().sortingOrder = 90;

        // hide the arrow
        ToggleSelectionArrow(false);
    }

    public void ToggleSelectionArrow(bool isActive)
    {
        _selectionArrow.gameObject.SetActive(isActive);
    }

    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        DeselectCharacter();

        // finish character's turn after the interaction is performed
        _turnManager.PlayerCharacterTurnFinished();
    }

    public void SetCharacterMoved(bool hasMoved)
    {
        HasMovedThisTurn = hasMoved;
    }
}

