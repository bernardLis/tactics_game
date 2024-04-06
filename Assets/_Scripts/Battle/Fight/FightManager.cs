using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Tiles;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Hero;
using Lis.Units.Minion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    public class FightManager : PoolManager<UnitController>
    {
        BattleManager _battleManager;
        AreaManager _areaManager;
        InputManager _inputManager;
        RangedOpponentManager _rangedOpponentManager;

        public Fight CurrentFight;

        [Header("Minion")] [SerializeField] GameObject _minionPrefab;

        readonly bool _debugSpawnMinion = false;

        HeroController _heroController;

        VisualElement _debugInfoContainer;
        Label _debugInfoLabel;

        IEnumerator _fightCoroutine;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
            _areaManager = _battleManager.GetComponent<AreaManager>();
            _inputManager = _battleManager.GetComponent<InputManager>();
            _rangedOpponentManager = _battleManager.GetComponent<RangedOpponentManager>();

#if UNITY_EDITOR
            _inputManager.OnRightMouseClick += DebugSpawnMinion;
            _inputManager.OnOneClicked += DebugSpawnRangedOpponent;
#endif
            SetupDebugInfo();

            CreatePool(_minionPrefab);

            _heroController = _battleManager.HeroController;

            CreateFight();
            _fightCoroutine = StartFightCoroutine();
            StartCoroutine(_fightCoroutine);

            _battleManager.OnTimeEnded += StopFight;
        }


        void SetupDebugInfo()
        {
            _debugInfoLabel = new Label();
            _debugInfoContainer = BattleManager.Instance.Root.Q<VisualElement>("fightDebugContainer");
            _debugInfoContainer.Add(_debugInfoLabel);
        }

        void CreateFight()
        {
            Fight fight = ScriptableObject.CreateInstance<Fight>();
            fight.CreateFight();
            CurrentFight = fight;
        }


        IEnumerator StartFightCoroutine()
        {
            yield return new WaitForSeconds(2f);

            while (true)
            {
                // HERE: testing
                if (_battleManager.IsGameLoopBlocked)
                {
                    yield return new WaitForSeconds(CurrentFight.DelayBetweenWaves);
                    continue;
                }

                if (_battleManager.BattleFinalized) yield break;

                SpawnWave();
                yield return new WaitForSeconds(CurrentFight.DelayBetweenWaves);
            }
        }

        void StopFight()
        {
            if (_fightCoroutine == null) return;
            StopCoroutine(_fightCoroutine);
        }

        void OnOpponentEntityDeath(UnitController _)
        {
        }

        public void SpawnWave()
        {
            EnemyWave wave = CurrentFight.GetCurrentWave();
            UpdateDebugInfo(wave);
            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        void UpdateDebugInfo(EnemyWave wave)
        {
            _debugInfoLabel.text =
                "Wave: " + wave.WaveIndex + " Points: " + wave.Points + " Level: " + wave.MinionLevel;
        }

        IEnumerator SpawnWaveCoroutine(EnemyWave wave)
        {
            yield return SpawnMinions(wave);
            if (wave.RangedOpponent != null)
                SpawnRangedOpponent(wave.RangedOpponent);
            CurrentFight.SpawningWaveFinished();
            yield return null;
        }

        IEnumerator SpawnMinions(EnemyWave wave)
        {
            foreach (Minion m in wave.Minions)
            {
                Vector3 pos =
                    _areaManager
                        .GetRandomPositionInRangeOnActiveTile(_heroController.transform.position,
                            Random.Range(20, 40));

                SpawnMinion(m, pos);
                yield return new WaitForSeconds(0.05f);
            }
        }

        void SpawnMinion(Unit m, Vector3 pos)
        {
            UnitController be = GetObjectFromPool();
            if (be == null)
            {
                Debug.LogError("No more minions in pool");
                return;
            }

            be.transform.position = pos;
            be.gameObject.SetActive(true);
            be.InitializeUnit(m, 1);
            _battleManager.AddOpponentArmyEntity(be);
        }

        void SpawnRangedOpponent(Unit unit)
        {
            Vector3 pos = _areaManager.GetRandomPositionInRangeOnActiveTile(_heroController.transform.position,
                Random.Range(20, 40));
            _rangedOpponentManager.SpawnRangedOpponent(unit, pos);
        }

#if UNITY_EDITOR

        int _debugMinionIndex;
        Camera _cam;

        public void DebugSpawnMinion()
        {
            if (!_debugSpawnMinion) return;

            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                return;
            List<Minion> minions = GameManager.Instance.UnitDatabase.GetAllMinions();
            Minion m = Instantiate(minions[Random.Range(0, minions.Count)]);
            m.InitializeBattle(1);
            SpawnMinion(m, hit.point);
        }

        void DebugSpawnRangedOpponent()
        {
            if (!_debugSpawnMinion) return;

            if (_rangedOpponentManager == null) return;
            Unit instance = Instantiate(GameManager.Instance.UnitDatabase.GetRandomRangedOpponent());
            SpawnRangedOpponent(instance);
        }
#endif
    }
}