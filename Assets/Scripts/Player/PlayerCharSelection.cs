using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class PlayerCharSelection : CharacterSelection
{

    [HideInInspector] public bool hasMovedThisTurn;
    [HideInInspector] public bool hasFinishedTurn;

    [HideInInspector] public Vector3 positionTurnStart;
    [HideInInspector] public WorldTile tileTurnStart;

    public Color grayOutColor;

    OscilateScale oscilateScale;
    SpriteRenderer[] spriteRenderers;


    public override void Awake()
    {
        base.Awake();
        FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnEnemyTurnEnd;
        FindObjectOfType<TurnManager>().playerTurnEndEvent += OnPlayerTurnEnd;


        // subscribe to player death
        GetComponent<CharacterStats>().characterDeathEvent += OnPlayerCharDeath;

        oscilateScale = GetComponentInChildren<OscilateScale>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    void Update()
    {
        // Highlight characters that have a move/finish their turn this turn and change their scale back and forth
        //if ((!hasMovedThisTurn && !hasFinishedTurn) && Time.frameCount % 60 == 0)
        //OscilateScale();
    }


    public bool CanBeSelected()
    {
        return !hasFinishedTurn;
    }

    public override void FinishCharacterTurn()
    {
        // stop playing "idle animation"
        oscilateScale.SetOscilation(false);

        GrayOutCharacter();

        hasFinishedTurn = true;
    }

    void GrayOutCharacter()
    {
        foreach (SpriteRenderer rend in spriteRenderers)
        {        
            if(rend != null)
                rend.DOColor(grayOutColor, 1f);
        }
    }

    void ReturnCharacterColor()
    {
        
        foreach (SpriteRenderer rend in spriteRenderers)
        {
            // well, shieet that looks way better than changing it right away;
            if(rend != null)
                rend.DOColor(Color.white, 2f);
        }

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

