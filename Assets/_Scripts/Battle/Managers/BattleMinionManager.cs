using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleMinionManager : PoolManager<BattleEntity>
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;
        BattleInputManager _battleInputManager;

        public int CurrentDifficulty { get; private set; }

        Fight _currentFight;

        [SerializeField] GameObject _minionPrefab;

        bool _debugSpawnMinion = true;

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

                SpawnWave();
                yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
            }
        }

        void OnOpponentEntityDeath(BattleEntity _)
        {
            if (_battleManager.OpponentEntities.Count != 0) return;
            if (!_currentFight.IsFinished()) return;
        }

        public void SpawnWave()
        {
            EnemyWave wave = _currentFight.EnemyWaves[_currentFight.CurrentWaveIndex];
            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        IEnumerator SpawnWaveCoroutine(EnemyWave wave)
        {
            // spawn minions on tiles next to the player or on the same tile
            List<BattleTile> tiles = _battleAreaManager.GetTilesAroundPlayer();
            yield return SpawnMinions(wave, tiles);
            _currentFight.SpawningWaveFinished();
        }

        public IEnumerator SpawnMinions(EnemyWave wave, List<BattleTile> tiles)
        {
            BattleTile tile = tiles[Random.Range(0, tiles.Count)];
            for (int i = 0; i < wave.Minions.Count; i++)
            {
                Minion m = wave.Minions[i];
                Vector3 pos = tile.GetMinionPosition(i, wave.Minions.Count);
                SpawnMinion(m, pos);
                yield return new WaitForSeconds(0.05f);
            }

            // HERE: ranged enemies are commented out for now
            // for (int i = 0; i < wave.Creatures.Count; i++)
            // {
            //     Creature c = wave.Creatures[i];
            //     c.InitializeBattle(1);

            //     Vector3 pos = tile.GetPositionRandom(i, wave.Creatures.Count);

            //     BattleEntity be = SpawnEntity(c, pos);
            //     be.InitializeEntity(c, 1);
            //     be.transform.position = pos;
            //     be.gameObject.SetActive(true);
            //     _battleManager.AddOpponentArmyEntity(be);
            //     yield return new WaitForSeconds(0.05f);
            // }
        }

        void SpawnMinion(Minion m, Vector3 pos)
        {
            m.InitializeBattle(1);
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

        BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
        {
            GameObject instance = Instantiate(entity.Prefab, spawnPos, transform.localRotation);
            BattleEntity be = instance.GetComponent<BattleEntity>();
            return be;
        }

        public void DebugSpawnMinion()
        {
            Debug.Log("DebugSpawnMinion");
            if (!_debugSpawnMinion) return;

            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            int layerMask = Tags.BattleFloorLayer;
            Minion m = _currentFight.EnemyWaves[0].Minions[0];
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
                SpawnMinion(m, hit.point);
        }
    }
}