using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Units;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class UnitTestManager : MonoBehaviour
    {
        public int TotalFights;
        public int FightsWon;
        public int FightsLost;

        public List<TestFight> Tests = new();
        private BattleManager _battleManager;
        private BreakableVaseManager _breakableVaseManager;

        private float _currentFightStartTime;
        private FightManager _fightManager;
        private TooltipManager _tooltipManager;

        private void Start()
        {
            _battleManager = BattleManager.Instance;
            _tooltipManager = TooltipManager.Instance;
            _fightManager = FightManager.Instance;
            _breakableVaseManager = _battleManager.GetComponent<BreakableVaseManager>();
            BattleInitializer.Instance.OnBattleInitialized += OnBattleInitialized;
        }

        private void OnBattleInitialized()
        {
            _tooltipManager.DisplayGameInfo(new Label("Unit tests engaged..."));
            Time.timeScale = 3f;
            _breakableVaseManager.DisableVaseSpawning();

            _fightManager.OnFightEnded += OnFightEnded;
            _fightManager.PlayerUnits.Clear(); // removing hero
            _fightManager.IsTesting = true;
            StartCoroutine(StartNewTestCoroutine());
        }

        private IEnumerator StartNewTestCoroutine()
        {
            yield return new WaitForSeconds(3f);
            ClearArmies();
            yield return new WaitForSeconds(3f);
            StartCoroutine(StartRandomTest());
        }

        private void ClearArmies()
        {
            _tooltipManager.DisplayGameInfo(new Label("Clearing armies..."));

            foreach (UnitController uc in new List<UnitController>(_fightManager.PlayerUnits))
                uc.Die();

            foreach (UnitController uc in new List<UnitController>(_fightManager.EnemyUnits))
                uc.Die();

            _fightManager.PlayerUnits.Clear();
            _fightManager.EnemyUnits.Clear();
        }

        private IEnumerator StartRandomTest()
        {
            TotalFights++;

            int points = Random.Range(100, 10000);
            TestFight test = ScriptableObject.CreateInstance<TestFight>();
            test.CreateTestFight(points);
            Tests.Add(test);
            yield return new WaitForSeconds(1f);

            foreach (Unit u in test.PlayerArmy)
                _fightManager.SpawnPlayerUnit(u);
            foreach (Unit u in test.EnemyArmy)
                _fightManager.SpawnEnemyUnit(u);

            yield return new WaitForSeconds(1f);

            _currentFightStartTime = _battleManager.GetTime();
            _fightManager.DebugStartFight();
        }


        private void OnFightEnded()
        {
            if (Tests.Count == 0) return;
            TestFight test = Tests.Last();
            test.FightDuration = (int)(_battleManager.GetTime() - _currentFightStartTime);
            bool pw = _fightManager.PlayerUnits.Count > 0;
            test.PlayerWon = pw;

            List<Unit> survivors = new();
            if (pw)
            {
                FightsWon++;
                foreach (UnitController uc in _fightManager.PlayerUnits)
                    survivors.Add(Instantiate(uc.Unit));
            }
            else
            {
                FightsLost++;
                foreach (UnitController uc in _fightManager.EnemyUnits)
                    survivors.Add(Instantiate(uc.Unit));
            }

            test.SetSurvivors(survivors);

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