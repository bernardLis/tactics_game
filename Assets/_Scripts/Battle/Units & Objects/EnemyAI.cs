using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    protected Highlighter highlighter;

    protected Seeker seeker;
    protected AILerp aiLerp;
    protected EnemyCharSelection characterSelection;

    protected EnemyStats enemyStats;
    public bool amDead = false;

    protected GameObject targetCharacter;


    public Brain brain;


    protected virtual void Awake()
    {
        highlighter = Highlighter.instance;

        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();

        characterSelection = GetComponent<EnemyCharSelection>();

        // subscribe to your death
        enemyStats = GetComponent<EnemyStats>();
        enemyStats.CharacterDeathEvent += OnEnemyDeath;
    }

    protected virtual void OnEnemyDeath(GameObject _obj)
    {
        // maybe useful for trapping on the way

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

        Task task = brain.Interact();
        yield return new WaitUntil(() => task.IsCompleted);

        yield return new WaitForSeconds(0.5f);

        highlighter.ClearHighlightedTiles().GetAwaiter();
        yield return new WaitForSeconds(0.5f);
        characterSelection.FinishCharacterTurn();
        yield return true;
    }
}
