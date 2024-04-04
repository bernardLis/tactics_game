using System.Collections;
using Lis.Core;
using Lis.Units;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class BossManager : MonoBehaviour
    {
        AudioManager _audioManager;
        BattleManager _battleManager;

        Boss _selectedBoss;
        BossController _bossController;
        [SerializeField] Sound _bossSpawnSound;
        [SerializeField] GameObject _bossSpawnEffectPrefab;

        public void Initialize()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;

            _battleManager.OnTimeEnded += SpawnBoss;
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
            _audioManager.PlaySfx(_bossSpawnSound, _battleManager.HeroController.transform.position);
            Destroy(Instantiate(_bossSpawnEffectPrefab), 4f);

            yield return new WaitForSeconds(1.5f);

            UnitController be = Instantiate(_battleManager.Battle.Boss.Prefab).GetComponent<UnitController>();
            be.transform.position = Vector3.up * 2.5f;
            be.gameObject.SetActive(true);
            be.InitializeGameObject();
            be.InitializeUnit(_selectedBoss, 1);

            _bossController = be as BossController;
            _battleManager.AddOpponentArmyEntity(_bossController);

            yield return null;
        }
    }
}