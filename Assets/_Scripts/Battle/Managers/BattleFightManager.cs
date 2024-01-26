using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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
        [SerializeField] Minion _earthMinion;
        [SerializeField] Minion _fireMinion;
        [SerializeField] Minion _waterMinion;
        [SerializeField] Minion _windMinion;

        List<Minion> _availableEarthMinions = new();
        List<Minion> _availableFireMinions = new();
        List<Minion> _availableWaterMinions = new();
        List<Minion> _availableWindMinions = new();

        readonly bool _debugSpawnMinion = true;

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

        // HERE: so much repetition
        Minion GetMinionByElementName(ElementName elName)
        {
            Minion m = null;

            switch (elName)
            {
                case ElementName.Earth:
                {
                    m = HandleEarthMinion();
                    break;
                }
                case ElementName.Fire:
                {
                    m = HandleFireMinion();
                    break;
                }
                case ElementName.Water:
                {
                    m = HandleWaterMinion();
                    break;
                }
                case ElementName.Wind:
                {
                    m = HandleWindMinion();
                    break;
                }
            }
            return m;
        }

        Minion HandleEarthMinion()
        {
            if (_availableEarthMinions.Count == 0)
            {
                Minion m = Instantiate(_earthMinion);
                m.OnDeath += () => _availableEarthMinions.Add(m);
                return m;
            }

            Minion minion = _availableEarthMinions[0];
            _availableEarthMinions.RemoveAt(0);
            return minion;
        }

        Minion HandleFireMinion()
        {
            if (_availableFireMinions.Count == 0)
            {
                Minion m = Instantiate(_fireMinion);
                m.OnDeath += () => _availableFireMinions.Add(m);
                return m;
            }

            Minion minion = _availableFireMinions[0];
            _availableFireMinions.RemoveAt(0);
            return minion;
        }

        Minion HandleWaterMinion()
        {
            if (_availableWaterMinions.Count == 0)
            {
                Minion m = Instantiate(_waterMinion);
                m.OnDeath += () => _availableWaterMinions.Add(m);
                return m;
            }

            Minion minion = _availableWaterMinions[0];
            _availableWaterMinions.RemoveAt(0);
            return minion;
        }

        Minion HandleWindMinion()
        {
            if (_availableWindMinions.Count == 0)
            {
                Minion m = Instantiate(_windMinion);
                m.OnDeath += () => _availableWindMinions.Add(m);
                return m;
            }

            Minion minion = _availableWindMinions[0];
            _availableWindMinions.RemoveAt(0);
            return minion;
        }

        // BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
        // {
        //     GameObject instance = Instantiate(entity.Prefab, spawnPos, transform.localRotation);
        //     BattleEntity be = instance.GetComponent<BattleEntity>();
        //     return be;
        // }

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
            switch (_debugMinionIndex)
            {
                case 0:
                    SpawnMinion(GetMinionByElementName(ElementName.Earth), hit.point);
                    _debugMinionIndex++;
                    break;
                case 1:
                    SpawnMinion(GetMinionByElementName(ElementName.Fire), hit.point);
                    _debugMinionIndex++;
                    break;
                case 2:
                    SpawnMinion(GetMinionByElementName(ElementName.Water), hit.point);
                    _debugMinionIndex++;
                    break;
                case 3:
                    SpawnMinion(GetMinionByElementName(ElementName.Wind), hit.point);
                    _debugMinionIndex = 0;
                    break;
            }
        }
    }
}