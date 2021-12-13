using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCharSelection : CharacterSelection
{
    public bool hasMovedThisTurn { get; private set; }

    public Vector3 positionTurnStart { get; private set; }
   // public WorldTile tileTurnStart { get; private set; }

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
        // reseting flags on turn's end
        hasMovedThisTurn = false;
        hasFinishedTurn = false;

        // remember on which tile you start the turn on 
        positionTurnStart = transform.position;

       // if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
          //  tileTurnStart = _tile;
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
        TurnManager.instance.PlayerCharacterTurnFinished();
    }



    public void SetCharacterMoved(bool hasMoved)
    {
        hasMovedThisTurn = hasMoved;
    }



}

