using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    HighlightManager _highlighter;

    Seeker _seeker;
    AILerp _aiLerp;
    EnemyCharSelection _characterSelection;
    ObjectUI _damageUI;

    EnemyStats _enemyStats;

    Brain _brain;

    void Awake()
    {
        _highlighter = HighlightManager.Instance;

        _seeker = GetComponent<Seeker>();
        _aiLerp = GetComponent<AILerp>();
        _characterSelection = GetComponent<EnemyCharSelection>();
        _enemyStats = GetComponent<EnemyStats>();

        _damageUI = GetComponent<ObjectUI>();
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

        // wait for statuses to resolve. 
        // TODO: there is an argument to implement it differently:
        // status triggers should be async-await and everything in between too. 
        while (!_damageUI.IsQueueEmpty())
            await Task.Delay(25); //https://stackoverflow.com/questions/29089417/c-sharp-wait-until-condition-is-true

        await Task.Delay(500);
        await _brain.Select();
        await Task.Delay(500);
        await _brain.Move();
        await Task.Delay(500);

        // wait for character to reach destination
        while (!_aiLerp.reachedDestination)
            await Task.Delay(25);

        await Task.Delay(500);
        await _brain.Interact();
        if (_enemyStats.CurrentHealth <= 0) // if you die during your turn
            return;

        await Task.Delay(500);
        await _highlighter.ClearHighlightedTiles();

        await Task.Delay(500);
        _characterSelection.FinishCharacterTurn();
    }
}
