using System.Collections;
using System.Collections.Generic;
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

        int CurrentDifficulty { get; set; }

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

#if UNITY_EDITOR
            _battleInputManager.OnRightMouseClick += DebugSpawnMinion;
#endif

            CurrentDifficulty = 1;

            CreatePool(_minionPrefab);

            _battleHero = _battleManager.BattleHero;

            CreateFight();
            StartCoroutine(StartFight());
        }


        void CreateFight()
        {
            Fight fight = ScriptableObject.CreateInstance<Fight>();
            fight.CreateFight(CurrentDifficulty);
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
            // if (!_currentFight.IsFinished()) return;
            // if (_battleManager.OpponentEntities.Count != 0) return;
        }

        public void SpawnWave()
        {
            EnemyWave wave = _currentFight.GetCurrentWave();
            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        IEnumerator SpawnWaveCoroutine(EnemyWave wave)
        {
            // // spawn minions on tiles next to the player or on the same tile
            // List<BattleTile> tiles = _battleAreaManager.GetTilesAroundPlayer();
            yield return SpawnMinions(wave);
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

            be.InitializeEntity(m, 1);
            be.transform.position = pos;
            be.gameObject.SetActive(true);
            _battleManager.AddOpponentArmyEntity(be);
        }


        // BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
        // {
        //     GameObject instance = Instantiate(entity.Prefab, spawnPos, transform.localRotation);
        //     BattleEntity be = instance.GetComponent<BattleEntity>();
        //     return be;
        // }
#if UNITY_EDITOR

        int _debugMinionIndex;
        Camera _cam;

        public void DebugSpawnMinion()
        {
            Debug.Log("DebugSpawnMinion");
            if (!_debugSpawnMinion) return;

            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            int layerMask = Tags.BattleFloorLayer;

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask)) return;
            List<EnemyGroup> enemyGroups = _currentFight.GetCurrentWave().EnemyGroups;
            SpawnMinion(enemyGroups[Random.Range(0, enemyGroups.Count)].Minions[0], hit.point);
        }
    }
#endif
}