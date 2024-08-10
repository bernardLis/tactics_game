using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lis.Arena;
using Lis.Arena.Pickup;
using Lis.Units;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Other
{
    public class UnitTestManager : MonoBehaviour
    {
        public int TotalFights;
        public int FightsWon;
        public int FightsLost;

        public List<TestWave> Tests = new();
        BreakableVaseManager _breakableVaseManager;

        float _currentFightStartTime;
        FightManager _fightManager;
        TooltipManager _tooltipManager;

        void Start()
        {
            _tooltipManager = TooltipManager.Instance;
            _fightManager = FightManager.Instance;
            _breakableVaseManager = _fightManager.GetComponent<BreakableVaseManager>();
            ArenaInitializer.Instance.OnArenaInitialized += OnArenaInitialized;
        }

        void OnArenaInitialized()
        {
            _tooltipManager.DisplayGameInfo(new Label("Unit tests engaged..."));
            Time.timeScale = 3f;
            _breakableVaseManager.DisableVaseSpawning();

            _fightManager.OnFightEnded += OnFightEnded;
            _fightManager.PlayerUnits.Clear(); // removing hero
            _fightManager.IsTesting = true;
            StartCoroutine(StartNewTestCoroutine());
        }

        IEnumerator StartNewTestCoroutine()
        {
            yield return new WaitForSeconds(3f);
            ClearArmies();
            yield return new WaitForSeconds(3f);
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
            TotalFights++;

            int points = Random.Range(100, 10000);
            TestWave test = ScriptableObject.CreateInstance<TestWave>();
            test.CreateTestFight(points);
            Tests.Add(test);
            yield return new WaitForSeconds(1f);

            foreach (Unit u in test.PlayerArmy)
                _fightManager.SpawnPlayerUnit(u);
            foreach (Unit u in test.EnemyArmy)
                _fightManager.SpawnEnemyUnit(u.Id);

            yield return new WaitForSeconds(1f);

            _currentFightStartTime = _fightManager.GetTime();
            _fightManager.DebugStartFight();
        }


        void OnFightEnded()
        {
            if (Tests.Count == 0) return;
            TestWave test = Tests.Last();
            test.FightDuration = (int)(_fightManager.GetTime() - _currentFightStartTime);
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