using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    Highlighter _highlighter;

    Seeker _seeker;
    AILerp _AILerp;
    EnemyCharSelection _characterSelection;

    EnemyStats _enemyStats;
    [HideInInspector] public bool _amDead = false;

    Brain _brain;

    protected virtual void Awake()
    {
        _highlighter = Highlighter.Instance;

        _seeker = GetComponent<Seeker>();
        _AILerp = GetComponent<AILerp>();

        _characterSelection = GetComponent<EnemyCharSelection>();

        // subscribe to your death
        _enemyStats = GetComponent<EnemyStats>();
        _enemyStats.CharacterDeathEvent += OnEnemyDeath;
    }

    protected virtual void OnEnemyDeath(GameObject _obj)
    {
        // maybe useful for trapping on the way

        // it exits the coroutine Run()
        _amDead = true;
    }

    public void SetBrain(Brain brain)
    {
        _brain = brain;
    }

    // TODO: rewrite to async await
    public virtual IEnumerator RunAI()
    {
        // exit if battle is over
        if (TurnManager.BattleState == BattleState.Won || TurnManager.BattleState == BattleState.Lost)
            yield break;

        // character can be stunned = no turn
        if (_characterSelection.HasFinishedTurn)
            yield break;

        yield return new WaitForSeconds(0.5f);
        _brain.Select();
        yield return new WaitForSeconds(0.5f);
        _brain.Move();
        yield return new WaitForSeconds(0.5f);

        // wait for character to reach destination
        while (!_AILerp.reachedDestination)
            yield return null;
        yield return new WaitForSeconds(0.5f);

        Task task = _brain.Interact();
        yield return new WaitUntil(() => task.IsCompleted);

        yield return new WaitForSeconds(0.5f);

        _highlighter.ClearHighlightedTiles().GetAwaiter();
        yield return new WaitForSeconds(0.5f);
        _characterSelection.FinishCharacterTurn();
        yield return true;
    }
}
