using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    GameObject[] _enemies;
    EnemyAI _enemyAI;

    void Awake()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.EnemyTurn)
            StartCoroutine(ForEachEnemy());
    }

    IEnumerator ForEachEnemy()
    {
        // Recalculate all graphs
        //AstarPath.active.Scan();

        yield return new WaitForSeconds(1.5f); // wait for statuses and modifiers to resolve
        if (TurnManager.BattleState != BattleState.EnemyTurn)
            yield break;

        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // for every enemy character
        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;

            InfoCardUI.Instance.ShowCharacterCard(enemy.GetComponent<CharacterStats>());
            _enemyAI = enemy.GetComponent<EnemyAI>();
            // this waits until the previous corutine is done
            yield return StartCoroutine(_enemyAI.RunAI());

            yield return new WaitForSeconds(1f);
        }
    }
}
