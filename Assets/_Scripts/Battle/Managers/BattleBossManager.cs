using System.Collections;



using UnityEngine;

namespace Lis
{
    public class BattleBossManager : MonoBehaviour
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;

        [SerializeField] Building[] _bossBuildings;
        BattleTile _bossTile;

        Building _chosenBossBuilding;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
            // _battleAreaManager.OnBossTileUnlocked += SpawnBoss;

            _chosenBossBuilding = _bossBuildings[Random.Range(0, _bossBuildings.Length)];
        }

        public void SpawnBoss(BattleTile tile)
        {
            StartCoroutine(SpawnBossCoroutine(tile));
        }

        IEnumerator SpawnBossCoroutine(BattleTile tile)
        {
            _bossTile = _battleAreaManager.ReplaceTile(tile, _chosenBossBuilding);

            yield return new WaitForSeconds(1f);
            // _battleAreaManager.SecureNextTile(_bossTile);
            yield return new WaitForSeconds(1f);
            _bossTile.Unlock();
        }

    }
}
