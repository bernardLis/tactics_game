using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

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

    async void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.EnemyTurn)
            await ForEachEnemy();
    }

    async Task ForEachEnemy()
    {
        // TODO: if select resolves statuses, you don't need to wait for so long
        await Task.Delay(1500); // wait for statuses and modifiers to resolve
        if (TurnManager.BattleState != BattleState.EnemyTurn)
            return;

        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;

            InfoCardUI.Instance.ShowCharacterCard(enemy.GetComponent<CharacterStats>());
            _enemyAI = enemy.GetComponent<EnemyAI>();

            // this waits until the previous corutine is done
            await _enemyAI.RunAI();

            await Task.Delay(1000);
        }
    }
}
