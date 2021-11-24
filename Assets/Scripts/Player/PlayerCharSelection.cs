using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class PlayerCharSelection : CharacterSelection
{

    public bool hasMovedThisTurn { get; private set; }
    public bool hasFinishedTurn { get; private set; }

    public Vector3 positionTurnStart { get; private set; }
    public WorldTile tileTurnStart { get; private set; }

    public Color grayOutColor;
    public SelectionArrow selectionArrow;

    SpriteRenderer[] spriteRenderers;


    public override void Awake()
    {
        base.Awake();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn();

    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }


    public bool CanBeSelected()
    {
        return !hasFinishedTurn;
    }

    public void HandlePlayerTurn()
    {
        // reseting flags on turn's end
        hasMovedThisTurn = false;
        hasFinishedTurn = false;

        // remember on which tile you start the turn on 
        positionTurnStart = transform.position;

        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            tileTurnStart = _tile;
    }

    void HandleEnemyTurn()
    {
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
        DeselectCharacter();
        GrayOutCharacter();

        hasFinishedTurn = true;
    }

    void GrayOutCharacter()
    {
        foreach (SpriteRenderer rend in spriteRenderers)
        {
            if (rend != null)
                rend.DOColor(grayOutColor, 1f);
        }
    }

    void ReturnCharacterColor()
    {
        foreach (SpriteRenderer rend in spriteRenderers)
        {
            // shieet that looks way better than changing it right away;
            if (rend != null)
                rend.DOColor(Color.white, 2f);
        }
    }

    public void SetCharacterMoved(bool hasMoved)
    {
        hasMovedThisTurn = hasMoved;
    }



}

