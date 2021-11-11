using UnityEngine;
using DG.Tweening;

public class PlayerCharSelection : CharacterSelection
{

    [HideInInspector] public bool hasMovedThisTurn { get; private set; }
    [HideInInspector] public bool hasFinishedTurn { get; private set; }

    [HideInInspector] public Vector3 positionTurnStart { get; private set; }
    [HideInInspector] public WorldTile tileTurnStart { get; private set; }

    public Color grayOutColor;

    OscilateScale oscilateScale;
    public SelectionArrow selectionArrow;
    SpriteRenderer[] spriteRenderers;


    public override void Awake()
    {
        base.Awake();
        FindObjectOfType<TurnManager>().EnemyTurnEndEvent += OnEnemyTurnEnd;
        FindObjectOfType<TurnManager>().PlayerTurnEndEvent += OnPlayerTurnEnd;

        // subscribe to player death
        GetComponent<CharacterStats>().CharacterDeathEvent += OnPlayerCharDeath;

        oscilateScale = GetComponentInChildren<OscilateScale>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public bool CanBeSelected()
    {
        return !hasFinishedTurn;
    }

    public void SelectCharacter()
    {
        selectionArrow.gameObject.SetActive(true);
    }

    public void DeselectCharacter()
    {
        // hide the arrow
        selectionArrow.gameObject.SetActive(false);

    }

    public override void FinishCharacterTurn()
    {
        DeselectCharacter();
        // stop playing "idle animation"
        oscilateScale.SetOscilation(false);

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

    void OnPlayerCharDeath()
    {
    }

    void OnPlayerTurnEnd()
    {
        Invoke("ReturnCharacterColor", 1f);
    }

    public void OnEnemyTurnEnd()
    {
        oscilateScale.SetOscilation(true);

        // reseting flags on turn's end
        hasMovedThisTurn = false;
        hasFinishedTurn = false;

        // remember on which tile you start the turn on 
        positionTurnStart = transform.position;

        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            tileTurnStart = _tile;
    }

}

