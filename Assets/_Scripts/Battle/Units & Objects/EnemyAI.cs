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

    void Awake()
    {
        _highlighter = Highlighter.Instance;

        _seeker = GetComponent<Seeker>();
        _AILerp = GetComponent<AILerp>();

        _characterSelection = GetComponent<EnemyCharSelection>();

        // subscribe to your death
        _enemyStats = GetComponent<EnemyStats>();
        _enemyStats.CharacterDeathEvent += OnEnemyDeath;
    }

    void OnEnemyDeath(GameObject _obj)
    {
        // maybe useful for trapping on the way

        // it exits the coroutine Run()
        _amDead = true;
    }

    public void SetBrain(Brain brain)
    {
        _brain = brain;
    }

    public async Task RunAI()
    {
        // exit if battle is over
        if (TurnManager.BattleState == BattleState.Won || TurnManager.BattleState == BattleState.Lost)
            return;

        // character can be stunned = no turn
        if (_characterSelection.HasFinishedTurn)
            return;

        await Task.Delay(500);
        await _brain.Select();
        await Task.Delay(500);
        _brain.Move();
        await Task.Delay(500);

        // wait for character to reach destination
        while (!_AILerp.reachedDestination)
            await Task.Yield();

        await Task.Delay(500);
        await _brain.Interact();

        await Task.Delay(500);
        await _highlighter.ClearHighlightedTiles();

        await Task.Delay(500);
        _characterSelection.FinishCharacterTurn();

        return;
    }
}
