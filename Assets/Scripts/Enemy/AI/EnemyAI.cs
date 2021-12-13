using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    protected Seeker seeker;
    protected AILerp aiLerp;
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

    // TODO: rewrite to async await
    public virtual IEnumerator RunAI()
    {
        // exit if battle is over
        if (TurnManager.battleState == BattleState.Won || TurnManager.battleState == BattleState.Lost)
            yield break;

        // character can be stunned = no turn
        if (characterSelection.hasFinishedTurn)
            yield return true;


        yield return new WaitForSeconds(0.5f);
        brain.Select();
        yield return new WaitForSeconds(0.5f);
        brain.Move();
        yield return new WaitForSeconds(0.5f);

        // wait for character to reach destination
        while (!aiLerp.reachedDestination)
            yield return null;
        yield return new WaitForSeconds(0.5f);

        Task task = brain.Interact();
        yield return new WaitUntil(() => task.IsCompleted);

        yield return new WaitForSeconds(0.5f);

        // need to wait for retaliation... wooow...
        if (brain.target == null)
        {
            characterSelection.FinishCharacterTurn();
            yield return true;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        characterSelection.FinishCharacterTurn();
        yield return true;
    }
}
