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

        BossController _bossController;
        [SerializeField] Sound _bossSpawnSound;
        [SerializeField] GameObject _bossSpawnEffectPrefab;

        public void Initialize()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
            _battleManager.GetComponent<TooltipManager>().ShowGameInfo("Boss is Spawned!", 2f);

            _audioManager.PlaySfx(_bossSpawnSound, _battleManager.HeroController.transform.position);
            Destroy(Instantiate(_bossSpawnEffectPrefab), 4f);

            yield return new WaitForSeconds(1.5f);
            Boss boss = _battleManager.Battle.Boss;
            UnitController be = Instantiate(boss.Prefab).GetComponent<UnitController>();
            be.transform.position = Vector3.up * 2.5f;
            be.gameObject.SetActive(true);
            be.InitializeGameObject();
            be.InitializeUnit(boss, 1);

            _bossController = be as BossController;
            _battleManager.AddOpponentArmyEntity(_bossController);

            yield return null;
        }
    }
}