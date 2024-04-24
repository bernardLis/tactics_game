using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    using Arena;

    public class FightManager : MonoBehaviour
    {
        ArenaManager _arenaManager;
        public Transform EntityHolder;

        Battle _battle;
        Arena _arena;

        [HideInInspector] public Fight CurrentFight;

        Label _timerLabel;
        VisualElement _debugInfoContainer;
        Label _debugInfoLabel;

        HeroController _heroController;

        public List<UnitController> PlayerUnits = new();
        public List<UnitController> EnemyUnits = new();

        public List<UnitController> KilledPlayerUnits = new();
        public List<Unit> KilledEnemyUnits = new();

        public event Action<UnitController> OnPlayerUnitAdded;
        public event Action<UnitController> OnPlayerUnitDeath;
        public event Action<UnitController> OnEnemyUnitAdded;
        public event Action<UnitController> OnEnemyUnitDeath;
        public event Action OnFightStarted;
        public event Action OnFightEnded;

        public bool IsFightActive { get; private set; }
        IEnumerator _fightCoroutine;
        int _fightTime;

        public void Initialize(Battle battle)
        {
            _battle = battle;
            _arena = _battle.CurrentArena;

            _arenaManager = GetComponent<ArenaManager>();
            _heroController = GetComponent<HeroManager>().HeroController;
            _heroController.Hero.OnArmyAdded += SpawnPlayerUnit;
            MoveHeroToSpawnPoint();

            SetupUi();
            CreateFight();
            StartCoroutine(SpawnAllPlayerUnits());
            StartCoroutine(SpawnEnemyUnits());

            // TODO: for now
            GetComponent<InputManager>().OnOneClicked += StartFight;
        }

        void MoveHeroToSpawnPoint()
        {
            _heroController.transform.position = _arenaManager.GetRandomPositionInPlayerLockerRoom();
        }

        void SetupUi()
        {
            VisualElement root = BattleManager.Instance.Root;
            _timerLabel = root.Q<Label>("timer");

            _debugInfoLabel = new Label();
            _debugInfoContainer = root.Q<VisualElement>("fightDebugContainer");
            _debugInfoContainer.Add(_debugInfoLabel);
        }

        void CreateFight()
        {
            CurrentFight = _arena.CreateFight();
        }

        public void StartFight()
        {
            _fightCoroutine = StartFightCoroutine();
            StartCoroutine(_fightCoroutine);

            StartCoroutine(TimerCoroutine());
        }

        IEnumerator TimerCoroutine()
        {
            _fightTime = 0;
            while (true)
            {
                if (this == null) yield break;

                _fightTime++;
                int minutes = Mathf.FloorToInt(_fightTime / 60f);
                int seconds = Mathf.FloorToInt(_fightTime - minutes * 60);

                _timerLabel.text = $"{minutes:00}:{seconds:00}";
                yield return new WaitForSeconds(1f);
            }
        }


        IEnumerator StartFightCoroutine()
        {
            if (IsFightActive) yield break;
            IsFightActive = true;

            UpdateDebugInfo();
            OnFightStarted?.Invoke();
        }

        IEnumerator SpawnAllPlayerUnits()
        {
            foreach (Creature c in _heroController.Hero.Army)
            {
                SpawnPlayerUnit(c);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void SpawnPlayerUnit(Creature c)
        {
            Vector3 pos = _arenaManager.GetRandomPositionInPlayerLockerRoom();
            GameObject g = Instantiate(c.Prefab, pos, Quaternion.identity, EntityHolder);
            UnitController unitController = g.GetComponent<UnitController>();
            unitController.InitializeGameObject();
            unitController.InitializeUnit(c, 0);
            CreatureController cc = unitController as CreatureController;
            if (cc == null) return;
            cc.SetOpponentList(ref EnemyUnits);
            AddPlayerUnit(unitController);
        }

        IEnumerator SpawnEnemyUnits()
        {
            foreach (Creature c in _arena.Fights.Last().OpponentArmy)
            {
                c.InitializeBattle(1);
                Vector3 pos = _arenaManager.GetRandomPositionInEnemyLockerRoom();
                GameObject g = Instantiate(c.Prefab, pos, Quaternion.identity, EntityHolder);
                UnitController unitController = g.GetComponent<UnitController>();
                unitController.InitializeGameObject();
                unitController.InitializeUnit(c, 1);
                CreatureController cc = unitController as CreatureController;
                if (cc == null) continue;
                cc.SetOpponentList(ref PlayerUnits);
                AddEnemyUnit(unitController);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void AddPlayerUnit(UnitController b)
        {
            b.transform.parent = EntityHolder;
            PlayerUnits.Add(b);
            b.OnDeath += PlayerUnitDies;
            if (b is CreatureController creature)
                OnPlayerUnitAdded?.Invoke(creature);
        }

        public void AddEnemyUnit(UnitController b)
        {
            EnemyUnits.Add(b);
            b.OnDeath += EnemyUnitDies;
            OnEnemyUnitAdded?.Invoke(b);
        }

        void PlayerUnitDies(UnitController be, UnitController killer)
        {
            KilledPlayerUnits.Add(be);
            PlayerUnits.Remove(be);
            OnPlayerUnitDeath?.Invoke(be);
        }

        void EnemyUnitDies(UnitController be, UnitController killer)
        {
            KilledEnemyUnits.Add(be.Unit);
            EnemyUnits.Remove(be);
            OnEnemyUnitDeath?.Invoke(be);

            if (IsFightActive && EnemyUnits.Count == 0)
                EndFight();
        }

        void EndFight()
        {
            _arena.CreateFight();
            StartCoroutine(SpawnEnemyUnits());
            RespawnPlayerUnits();
            // MoveHeroToSpawnPoint();
            OnFightEnded?.Invoke();
            IsFightActive = false;

            // ClearUnits(); // TODO: for now

            FightRewardScreen rewardScreen = new();


            // rewardScreen.OnHide += StartFight;
        }

        void RespawnPlayerUnits()
        {
            foreach (UnitController c in KilledPlayerUnits)
            {
                if (c is not CreatureController) continue;
                CreatureController cc = c as CreatureController;
                cc.Respawn();
            }

            KilledPlayerUnits.Clear();
        }

        public List<UnitController> GetAllies(UnitController unitController)
        {
            return unitController.Team == 0 ? PlayerUnits : EnemyUnits;
        }

        public List<UnitController> GetOpponents(int team)
        {
            return team == 0 ? EnemyUnits : PlayerUnits;
        }

        public Vector3 GetRandomEnemyPosition()
        {
            return EnemyUnits.Count == 0
                ? Vector3.zero
                : EnemyUnits[Random.Range(0, EnemyUnits.Count)].transform.position;
        }

        void UpdateDebugInfo()
        {
            // TODO: Update debug info
            // _debugInfoLabel.text =
            //     "Wave: " + wave.WaveIndex + " Points: " + wave.Points + " Level: " + wave.MinionLevel;
        }
    }
}