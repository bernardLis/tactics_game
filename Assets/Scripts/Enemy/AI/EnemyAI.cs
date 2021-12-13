using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    protected Seeker seeker;
    protected AILerp aiLerp;
    protected Highlighter highlighter;
    protected EnemyCharSelection characterSelection;

    protected EnemyStats enemyStats;
    public bool amDead = false;

    protected GameObject targetCharacter;

    // tilemap vars
    protected Dictionary<Vector3, WorldTile> tiles;
    protected Tilemap tilemap;
    protected WorldTile _tile;
    protected WorldTile currentTile;
    protected WorldTile targetTile;

    protected bool targetInRange;
    protected GameObject[] playerCharacters;
    protected int abilityRange;

    public Brain brain;


    protected virtual void Awake()
    {
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();

        highlighter = Highlighter.instance;
        characterSelection = GetComponent<EnemyCharSelection>();

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        // subscribe to your death
        enemyStats = GetComponent<EnemyStats>();
        enemyStats.CharacterDeathEvent += OnEnemyDeath;
    }

    protected virtual void OnEnemyDeath()
    {
        // it exits the coroutine Run()
        amDead = true;
    }

    public virtual IEnumerator RunAI()
    {
        // exit if battle is over
        if (TurnManager.battleState == BattleState.Won || TurnManager.battleState == BattleState.Lost)
            yield break;

        yield return new WaitForSeconds(0.5f);
        brain.Select();
        yield return new WaitForSeconds(0.5f);
        brain.Move();
        yield return new WaitForSeconds(0.5f);
        
        // wait for character to reach destination
        while (!aiLerp.reachedDestination)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        brain.Interact();
        // need to wait for retaliation... wooow...
        yield return new WaitForSeconds(1.5f); // TODO: I think it may be hard to do it properly
        highlighter.ClearHighlightedTiles().GetAwaiter();

        TurnManager.instance.EnemyCharacterTurnFinished();
        yield return true;
    }
}
