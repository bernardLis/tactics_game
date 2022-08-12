using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
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
        await Task.Delay(500);
        if (TurnManager.BattleState != BattleState.EnemyTurn)
            return;

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;

            await enemy.GetComponent<EnemyAI>().RunAI();

            await Task.Delay(500);
        }
    }
}
