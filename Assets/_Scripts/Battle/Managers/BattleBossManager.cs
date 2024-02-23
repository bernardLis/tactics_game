using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossManager : MonoBehaviour
    {
        BattleManager _battleManager;

        Boss _selectedBoss;
        BattleBoss _battleBoss;

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

            BattleEntity be = Instantiate(_selectedBoss.Prefab).GetComponent<BattleEntity>();
            be.transform.position = Vector3.up * 2.5f;
            be.gameObject.SetActive(true);
            be.InitializeGameObject();
            be.InitializeEntity(_selectedBoss, 1);

            _battleBoss = be as BattleBoss;
            _battleManager.AddOpponentArmyEntity(_battleBoss);

            yield return null;
        }
    }
}