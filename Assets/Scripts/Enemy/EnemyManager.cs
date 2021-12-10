using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemies;
    EnemyAI enemyAI;

    public static EnemyManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Enemy Manager found");
            return;
        }
        instance = this;
        #endregion

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.EnemyTurn)
            StartCoroutine(ForEachEnemy());
    }

    IEnumerator ForEachEnemy()
    {
        // TODO: is this ok, performance-wise?
        // but it fixes a problem where enemies did not have updated graph
        // after player moved stone at the end of player turn
        // Recalculate all graphs
        AstarPath.active.Scan();

        yield return new WaitForSeconds(1.5f);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // for every enemy character
        foreach (var enemy in enemies)
        {
            if (enemy == null)
                continue;
            
            BasicCameraFollow.instance.followTarget = enemy.transform;
            InfoCardUI.instance.ShowCharacterCard(enemy.GetComponent<CharacterStats>());
            enemyAI = enemy.GetComponent<EnemyAI>();
            // this waits until the previous corutine is done
            yield return StartCoroutine(enemyAI.RunAI());

            yield return new WaitForSeconds(1f);
        }
    }
}
