using System.Collections;
using Lis.Core;
using Lis.Units;
using Lis.Units.Boss;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Fight
{
    public class BossManager : MonoBehaviour
    {
        [SerializeField] Sound _bossSpawnSound;
        [SerializeField] GameObject _bossSpawnEffectPrefab;
        AudioManager _audioManager;
        BattleManager _battleManager;

        BossController _bossController;
        FightManager _fightManager;
        HeroManager _heroManager;


        public void Initialize()
        {
            _audioManager = AudioManager.Instance;
            _battleManager = BattleManager.Instance;
            _heroManager = GetComponent<HeroManager>();
            _fightManager = GetComponent<FightManager>();
        }

        public void SpawnBoss()
        {
            StartCoroutine(SpawnBossCoroutine());
        }

        IEnumerator SpawnBossCoroutine()
        {
            _battleManager.GetComponent<TooltipManager>().DisplayGameInfo(new Label("Boss is Spawned!"));

            _audioManager.PlaySfx(_bossSpawnSound, _heroManager.transform.position);
            Destroy(Instantiate(_bossSpawnEffectPrefab), 4f);

            yield return new WaitForSeconds(1.5f);
            Boss boss = _battleManager.Battle.CurrentArena.Boss;
            UnitController be = Instantiate(boss.Prefab).GetComponent<UnitController>();
            be.transform.position = Vector3.up * 2.5f;
            be.gameObject.SetActive(true);
            be.InitializeGameObject();
            be.InitializeUnit(boss, 1);

            _bossController = be as BossController;
            _fightManager.AddEnemyUnit(_bossController);

            yield return null;
        }
    }
}