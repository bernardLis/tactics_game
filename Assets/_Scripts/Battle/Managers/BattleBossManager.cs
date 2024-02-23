using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossManager : MonoBehaviour
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;

        Boss _selectedBoss;
        BattleBoss _battleBoss;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

            _selectedBoss = Instantiate(GameManager.Instance.EntityDatabase.GetRandomBoss());
            _selectedBoss.InitializeBattle(1);
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
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