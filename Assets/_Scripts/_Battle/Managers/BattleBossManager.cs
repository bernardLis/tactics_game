using System.Collections;
using Lis.Core;
using Lis.Units;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis
{
    public class BattleBossManager : MonoBehaviour
    {
        BattleManager _battleManager;

        Boss _selectedBoss;
        BossController _bossController;

        [SerializeField] GameObject _bossSpawnEffectPrefab;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;

            _selectedBoss = Instantiate(GameManager.Instance.EntityDatabase.GetRandomBoss());
            _selectedBoss.InitializeBattle(1);
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
            Destroy(Instantiate(_bossSpawnEffectPrefab), 4f);

            yield return new WaitForSeconds(1.5f);

            UnitController be = Instantiate(_selectedBoss.Prefab).GetComponent<UnitController>();
            be.transform.position = Vector3.up * 2.5f;
            be.gameObject.SetActive(true);
            be.InitializeGameObject();
            be.InitializeEntity(_selectedBoss, 1);

            _bossController = be as BossController;
            _battleManager.AddOpponentArmyEntity(_bossController);

            yield return null;
        }
    }
}