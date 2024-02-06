using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleBuildingBoss : BattleBuilding
    {
        [SerializeField] Transform _bossSpawnPoint;
        [SerializeField] Boss _bossOriginal;
        [SerializeField] BattleEntitySpawner _battleEntitySpawnerPrefab;

        BattleBoss _boss;

        public override void Initialize(Vector3 pos, Building building)
        {
            pos = Vector3.zero;
            base.Initialize(pos, building);
        }

        protected override IEnumerator ShowBuildingCoroutine()
        {
            yield return base.ShowBuildingCoroutine();
            yield return SpawnBossCoroutine();
        }

        IEnumerator SpawnBossCoroutine()
        {
            BattleEntitySpawner spawner = Instantiate(_battleEntitySpawnerPrefab,
                _bossSpawnPoint.position,
                Quaternion.identity);
            spawner.transform.LookAt(BattleManager.BattleHero.transform.position);
            spawner.ShowPortal(null, Vector3.one * 5f);
            yield return new WaitForSeconds(1.5f);

            Boss boss = Instantiate(_bossOriginal);
            spawner.SpawnEntities(new List<Entity> { boss }, team: 1);
            spawner.OnSpawnComplete += list =>
            {
                _boss = list[0] as BattleBoss;
                BattleManager.AddOpponentArmyEntity(_boss);
            };
        }
    }
}