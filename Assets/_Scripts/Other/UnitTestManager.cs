using System.Collections;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Units;
using Lis.Units.Enemy;
using Lis.Units.Peasant;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class UnitTestManager : MonoBehaviour
    {
        BattleManager _battleManager;
        TooltipManager _tooltipManager;
        FightManager _fightManager;
        BreakableVaseManager _breakableVaseManager;

        [SerializeField] Peasant _peasant;
        [SerializeField] List<Enemy> _enemies = new();

        public List<string> Log = new();

        void Start()
        {
            _battleManager = BattleManager.Instance;
            _tooltipManager = TooltipManager.Instance;
            _fightManager = FightManager.Instance;
            _breakableVaseManager = _battleManager.GetComponent<BreakableVaseManager>();
            BattleInitializer.Instance.OnBattleInitialized += OnBattleInitialized;
        }

        void OnBattleInitialized()
        {
            _tooltipManager.DisplayGameInfo(new Label("Unit tests engaged..."));
            _breakableVaseManager.DisableVaseSpawning();

            _fightManager.OnFightEnded += OnFightEnded;
            _fightManager.PlayerUnits.Clear(); // removing hero
            StartCoroutine(StartNewTestCoroutine());
        }

        IEnumerator StartNewTestCoroutine()
        {
            yield return new WaitForSeconds(3f);
            ClearArmies();
            yield return new WaitForSeconds(2f);
            StartCoroutine(StartRandomTest());
        }

        void ClearArmies()
        {
            _tooltipManager.DisplayGameInfo(new Label("Clearing armies..."));

            foreach (UnitController uc in new List<UnitController>(_fightManager.PlayerUnits))
                uc.Die();

            foreach (UnitController uc in new List<UnitController>(_fightManager.EnemyUnits))
                uc.Die();

            _fightManager.PlayerUnits.Clear();
            _fightManager.EnemyUnits.Clear();
        }

        IEnumerator StartRandomTest()
        {
            _tooltipManager.DisplayGameInfo(new Label("Setting up new test..."));

            // add random amount of peasants to player army
            int peasantsCount = Random.Range(1, 10);
            for (int i = 0; i < peasantsCount; i++)
            {
                Peasant peasant = Instantiate(_peasant);
                peasant.InitializeBattle(0);
                _fightManager.SpawnPlayerUnit(peasant);
            }

            // add random amount of enemies to enemy army
            int enemiesCount = Random.Range(1, 10);
            for (int i = 0; i < enemiesCount; i++)
            {
                Enemy enemy = Instantiate(_enemies[Random.Range(0, _enemies.Count)]);
                enemy.InitializeBattle(1);
                _fightManager.SpawnEnemyUnit(enemy);
            }

            Log.Add($"{_battleManager.GetTime()}: Test started: {peasantsCount} peasants vs {enemiesCount} enemies");

            yield return new WaitForSeconds(2f);
            _fightManager.DebugStartFight();
        }

        void OnFightEnded()
        {
            string result = _fightManager.PlayerUnits.Count == 0 ? "Player lost, " : "Player won, ";
            string armyLeft = "army left: ";
            // it'd be better to aggregate units by type
            foreach (UnitController uc in _fightManager.PlayerUnits)
                armyLeft += uc.Unit.name + ", ";

            foreach (UnitController uc in _fightManager.EnemyUnits)
                armyLeft += uc.Unit.name + ", ";

            Log.Add(_battleManager.GetTime() + ": " + result + armyLeft);

            StartCoroutine(StartNewTestCoroutine());
        }


#if UNITY_EDITOR
        [Button]
        public void StopTests()
        {
            Debug.LogError("not implemented yet");
        }

        [Button]
        public void ExportLog()
        {
            Debug.LogError("not implemented yet");
        }
#endif
    }
}