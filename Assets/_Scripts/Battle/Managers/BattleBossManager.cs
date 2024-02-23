using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossManager : MonoBehaviour
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;


        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
            yield return null;
        }
    }
}