using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class EnemyCharMovementController : CharacterMovementController
{
    public EnemyStats myStats;
    EnemyCharSelection characterSelection;
    CharacterInteractionController characterInteractionController;

    WorldTile _tile;
    WorldTile currentTile;
    WorldTile destinationTile;
    GameObject targetPlayerCharacter;
    List<WorldTile> tilesWithinRange;
    GameObject tempObject;
    Transform myDestination;
    public bool reachedDestinationThisTurn = false;
    public bool trappedOnTheWay = false;

    protected override void Awake()
    {
        base.Awake();

        myStats = GetComponent<EnemyStats>();
        myStats.CharacterDeathEvent += OnEnemyCharDeath;

        characterSelection = (EnemyCharSelection)transform.GetComponent<EnemyCharSelection>();
        characterInteractionController = (CharacterInteractionController)transform.GetComponent<CharacterInteractionController>();
    }

    // TODO: I don't think this is the right place to do that
    // it is increasing the spaggettiness of my code.
    public void TrappedOnTheWay(int damage, Vector3 trapPosition)
    {
        // get the tile with trap object
        Vector3Int tilePos = tilemap.WorldToCell(tr.position);
        // ref coz I want to change the value
        if (tiles.TryGetValue(tilePos, out _tile))
        {
            if (!trappedOnTheWay)
            {
                trappedOnTheWay = true;

                if (tempObject != null)
                {
                    tempObject.transform.position = trapPosition;
                }
                else
                {
                    // this is our tile
                    tempObject = new GameObject("Enemy Destination");
                    // I want it to be in the center of the tile, not on the edge.
                    tempObject.transform.position = trapPosition;
                }

                AILerp.speed = 1f;

                destinationSetter.target = tempObject.transform;

                myStats.TakePiercingDamage(damage);
                // movement range is down by 1 for each trap enemy walks on
                myStats.movementRange.AddModifier(-1);
            }
        }
    }

    public void GoToDestination(Vector3 destination)
    {
        // clear highlight
        highlighter.ClearHighlightedTiles();

        // reset ailerp speed
        AILerp.speed = 3f;

        // reset trapped variable
        trappedOnTheWay = false;

        // move if destination is not the same as current position
        if (destination != transform.position)
        {
            tempObject = new GameObject("Enemy Destination");
            // I want it to be in the center of the tile, not on the edge.
            tempObject.transform.position = new Vector3(destination.x + 0.5f, destination.y + 0.5f, destination.z);

            destinationTile = highlighter.HighlightSingle(tempObject.transform.position, new Color(0.53f, 0.52f, 1f, 1f));

            // move
            destinationSetter.target = tempObject.transform;
            AILerp.canSearch = true;
            AILerp.canMove = true;
        }
        else
        {
            OnTargetReached();
        }
    }

    public override void OnTargetReached()
    {
        base.OnTargetReached();

        // reset ailerp speed
        AILerp.speed = 3f;

        reachedDestinationThisTurn = true;

        // clear the destination tile
        // TODO: is this unnecessary?
        //destinationTile.Highlight(Color.white);

        if (tempObject != null)
        {
            Destroy(tempObject);
        }
    }

    void OnEnemyCharDeath()
    {
        if (tempObject != null)
        {
            Destroy(tempObject);
        }
    }
}
