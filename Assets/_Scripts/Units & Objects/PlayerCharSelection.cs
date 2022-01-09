using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCharSelection : CharacterSelection
{
    public bool hasMovedThisTurn { get; private set; }

    public Vector3 positionTurnStart { get; private set; }

    public SelectionArrow selectionArrow;

    public override void Awake()
    {
        base.Awake();

    }

    public bool CanBeSelected()
    {
        return !hasFinishedTurn;
    }

    protected override void HandlePlayerTurn()
    {
        if (!myStats.isStunned)
        {
            hasMovedThisTurn = false;
            SetHasFinishedTurn(false);
        }
        else
            FinishCharacterTurn();

        // remember on which tile you start the turn on 
        positionTurnStart = transform.position;
    }

    protected override void HandleEnemyTurn()
    {
        if (!myStats.isStunned)
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
        selectionArrow.gameObject.SetActive(isActive);
    }

    public override void FinishCharacterTurn()
    {
        base.FinishCharacterTurn();
        DeselectCharacter();

        // finish character's turn after the interaction is performed
        turnManager.PlayerCharacterTurnFinished();
    }

    public void SetCharacterMoved(bool hasMoved)
    {
        hasMovedThisTurn = hasMoved;
    }
}

