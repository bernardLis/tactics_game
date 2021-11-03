using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharSelection : CharacterSelection
{

    public bool hasMovedThisTurn;
    public bool hasFinishedTurn;

    public Vector3 positionTurnStart;
    public WorldTile tileTurnStart;
    bool scalingUp;

    public override void Awake()
    {
        base.Awake();
        FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnEnemyTurnEnd;

        // subscribe to player death
        GetComponent<CharacterStats>().characterDeathEvent += OnPlayerCharDeath;
    }
    void Update()
    {
        // Highlight characters that have a move this turn and change their scale back and forth\
        if (!hasMovedThisTurn && Time.frameCount % 60 == 0)
            OscilateScale();
    }

    void OscilateScale()
    {
        if (scalingUp)
            transform.localScale += Vector3.one * 0.01f;
        else
            transform.localScale -= Vector3.one * 0.01f;

        if (transform.localScale.x > 1.03f)
            scalingUp = false;
        else if (transform.localScale.x < 0.98f)
            scalingUp = true;
    }

    void OnPlayerCharDeath()
    {
    }

    public void OnEnemyTurnEnd()
    {
        // reseting flags on turn's end
        hasMovedThisTurn = false;
        hasFinishedTurn = false;


        // remember on which tile you start the turn on 
        positionTurnStart = transform.position;

        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            tileTurnStart = _tile;
    }
}

