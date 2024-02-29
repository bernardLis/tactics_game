using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleFightManager : PoolManager<BattleEntity>
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;
        BattleInputManager _battleInputManager;
        BattleRangedOpponentManager _battleRangedOpponentManager;

        Fight _currentFight;

        [Header("Minion")] [SerializeField] GameObject _minionPrefab;

        readonly bool _debugSpawnMinion = true;

        BattleHero _battleHero;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
            _battleInputManager = _battleManager.GetComponent<BattleInputManager>();
            _battleRangedOpponentManager = _battleManager.GetComponent<BattleRangedOpponentManager>();

#if UNITY_EDITOR
            _battleInputManager.OnRightMouseClick += DebugSpawnMinion;
            _battleInputManager.OnOneClicked += DebugSpawnRangedOpponent;
#endif

            CreatePool(_minionPrefab);

            _battleHero = _battleManager.BattleHero;

            CreateFight();
            StartCoroutine(StartFight());
        }


        void CreateFight()
        {
            Fight fight = ScriptableObject.CreateInstance<Fight>();
            fight.CreateFight();
            _currentFight = fight;
        }

        IEnumerator StartFight()
        {
            yield return new WaitForSeconds(2f);

            while (true)
            {
                // HERE: testing
                if (_battleManager.IsGameLoopBlocked)
                {
                    yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
                    continue;
                }

                if (_battleManager.BattleFinalized) yield break;

                SpawnWave();
                yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
            }
        }

        void OnOpponentEntityDeath(BattleEntity _)
        {
        }

        public void SpawnWave()
        {
            EnemyWave wave = _currentFight.GetCurrentWave();
            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        IEnumerator SpawnWaveCoroutine(EnemyWave wave)
        {
            yield return SpawnMinions(wave);
            if (wave.RangedOpponent != null) SpawnRangedOpponent(wave.RangedOpponent);
            _currentFight.SpawningWaveFinished();
        }

        IEnumerator SpawnMinions(EnemyWave wave)
        {
            foreach (EnemyGroup group in wave.EnemyGroups)
            {
                foreach (Minion m in group.Minions)
                {
                    Vector3 pos =
                        _battleAreaManager
                            .GetRandomPositionInRangeOnActiveTile(_battleHero.transform.position,
                                Random.Range(20, 40));

                    SpawnMinion(m, pos);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        void SpawnMinion(Entity m, Vector3 pos)
        {
            BattleEntity be = GetObjectFromPool();
            if (be == null)
            {
                Debug.LogError("No more minions in pool");
                return;
            }

            be.transform.position = pos;
            be.gameObject.SetActive(true);
            be.InitializeEntity(m, 1);
            _battleManager.AddOpponentArmyEntity(be);
        }

        void SpawnRangedOpponent(Entity entity)
        {
            Vector3 pos = _battleAreaManager.GetRandomPositionInRangeOnActiveTile(_battleHero.transform.position,
                Random.Range(20, 40));
            _battleRangedOpponentManager.SpawnRangedOpponent(entity, pos);
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

            List<EnemyGroup> enemyGroups =
                _currentFight.EnemyWaves[Random.Range(0, _currentFight.EnemyWaves.Count)].EnemyGroups;
            SpawnMinion(enemyGroups[Random.Range(0, enemyGroups.Count)].Minions[0], hit.point);
        }

        void DebugSpawnRangedOpponent()
        {
            if (_battleRangedOpponentManager == null) return;
            Entity instance = Instantiate(GameManager.Instance.EntityDatabase.GetRandomRangedOpponent());
            SpawnRangedOpponent(instance);
        }
    }
#endif
}