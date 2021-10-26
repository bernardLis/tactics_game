using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharSelection : CharacterSelection
{
    public GameObject selectionCircle;
    SpriteRenderer selectionCircleRenderer;

    public bool movedThisTurn = false;

    public bool finishedTurn;
    bool scalingUp;


    public override void Awake()
    {
        base.Awake();
        FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnTurnEnd;
        selectionCircleRenderer = selectionCircle.GetComponent<SpriteRenderer>();

        // subscribe to player death
        GetComponent<CharacterStats>().characterDeathEvent += OnPlayerCharDeath;
    }
    void Update()
    {
        if (!finishedTurn && Time.frameCount % 45 == 0)
        {
            // Highlight characters that have a move this turn and change their scale back and forth\
            OscilateScale();
        }
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
        UnselectCharacter();
        highlighter.ClearHighlightedTiles();
    }

    public void SelectCharacter()
    {
        // selectionCircle.SetActive(true);
        if (!movedThisTurn)
        {
            selectionCircle.SetActive(false);
            HiglightMovementRange();
        }
    }
    public void UnselectCharacter()
    {
        //selectionCircle.SetActive(false);
        selectionCircle.SetActive(true);
        if (!movedThisTurn)
        {
            selectionCircleRenderer.color = new Color(0.53f, 0.52f, 1f, 1f);
        }
        else
        {
            selectionCircleRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

        highlighter.ClearHighlightedTiles();
    }

    public override void HiglightMovementRange()
    {
        base.HiglightMovementRange();
        highlighter.HiglightPlayerMovementRange(transform.position, range, new Color(0.53f, 0.52f, 1f, 1f));
    }

    public void OnTurnEnd()
    {
        if (selectionCircle != null)
            selectionCircleRenderer.color = new Color(0.53f, 0.52f, 1f, 1f);

        movedThisTurn = false;
    }
}
