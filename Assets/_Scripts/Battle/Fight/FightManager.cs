using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Attack;
using Lis.Units.Creature;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    using Arena;

    public class FightManager : Singleton<FightManager>
    {
        public Transform PlayerArmyHolder;
        [SerializeField] Transform _opponentArmyHolder;

        [HideInInspector] public Fight LastFight;
        [HideInInspector] public Fight CurrentFight;

        public List<UnitController> PlayerUnits = new();
        public List<UnitController> EnemyUnits = new();

        public List<UnitController> KilledPlayerUnits = new();
        public List<Unit> KilledEnemyUnits = new();
        public bool IsTesting;
        Arena _arena;

        ArenaManager _arenaManager;

        Battle _battle;

        IEnumerator _fightCoroutine;

        Label _fightInfoLabel;

        HeroController _heroController;
        TooltipManager _tooltipManager;
        public static bool IsFightActive { get; private set; }
        public static int FightNumber { get; private set; }

        public event Action<UnitController> OnPlayerUnitAdded;
        public event Action<UnitController> OnPlayerUnitDeath;
        public event Action<UnitController> OnEnemyUnitAdded;
        public event Action<UnitController> OnEnemyUnitDeath;
        public event Action OnFightStarted;
        public event Action OnFightEnded;

        public void Initialize(Battle battle)
        {
            _battle = battle;
            _arena = _battle.CurrentArena;

            _arenaManager = GetComponent<ArenaManager>();
            _heroController = GetComponent<HeroManager>().HeroController;
            _tooltipManager = GetComponent<TooltipManager>();

            StartCoroutine(SpawnAllPlayerUnits());

            IsFightActive = false;
            FightNumber = 0;

            _fightInfoLabel = new();
            BattleManager.Instance.Root.Q<VisualElement>("fightInfo").Add(_fightInfoLabel);
            UpdateFightInfoText();

            // HERE: testing
            GetComponent<InputManager>().OnOneClicked += StartFight;

            StartCoroutine(StartGame());
        }

        IEnumerator StartGame()
        {
            CurrentFight = _arena.CreateFight(_heroController.Hero.GetHeroPoints());
            CurrentFight.OnOptionChosen += SpawnEnemyArmy;
            CurrentFight.ChooseRandomOption();

            for (int i = 0; i < 3; i++)
            {
                _tooltipManager.DisplayGameInfo(new Label((3 - i).ToString()));
                yield return new WaitForSeconds(1f);
            }

            _heroController.StartAllAbilities();
            StartFight();
        }

        public void StartFight()
        {
            if (CurrentFight.ChosenOption == null) return;

            _fightCoroutine = StartFightCoroutine();
            StartCoroutine(_fightCoroutine);
        }

        public void DebugStartFight()
        {
            _fightCoroutine = StartFightCoroutine();
            StartCoroutine(_fightCoroutine);
        }

        IEnumerator StartFightCoroutine()
        {
            if (IsFightActive) yield break;
            IsFightActive = true;

            OnFightStarted?.Invoke();
        }

        IEnumerator SpawnAllPlayerUnits()
        {
            foreach (Unit u in _heroController.Hero.Army)
            {
                SpawnPlayerUnit(u);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public UnitController SpawnPlayerUnit(Unit u, Vector3 pos = default)
        {
            if (pos == default) pos = _arenaManager.GetRandomPositionInPlayerLockerRoom();
            GameObject g = Instantiate(u.Prefab, pos, Quaternion.identity, PlayerArmyHolder);
            UnitController unitController = g.GetComponent<UnitController>();
            unitController.InitializeGameObject();
            unitController.InitializeUnit(u, 0);
            unitController.SetOpponentList(ref EnemyUnits);
            AddPlayerUnit(unitController);

            return unitController;
        }

        void SpawnEnemyArmy(FightOption option)
        {
            CurrentFight.OnOptionChosen -= SpawnEnemyArmy;
            StartCoroutine(SpawnEnemyUnits(option.Army));
        }

        IEnumerator SpawnEnemyUnits(List<Unit> army)
        {
            foreach (Unit c in army)
            {
                SpawnEnemyUnit(c);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public UnitController SpawnEnemyUnit(Unit c)
        {
            Vector3 pos = _arenaManager.GetRandomPositionInEnemyLockerRoom();
            GameObject g = Instantiate(c.Prefab, pos, Quaternion.identity, _opponentArmyHolder);
            UnitController unitController = g.GetComponent<UnitController>();
            unitController.InitializeGameObject();
            unitController.InitializeUnit(c, 1);
            unitController.SetOpponentList(ref PlayerUnits);
            AddEnemyUnit(unitController);

            return unitController;
        }

        public void AddPlayerUnit(UnitController b)
        {
            b.transform.parent = PlayerArmyHolder;
            PlayerUnits.Add(b);
            b.OnDeath += PlayerUnitDies;
            if (b is CreatureController creature)
                OnPlayerUnitAdded?.Invoke(creature);
        }

        public void AddEnemyUnit(UnitController b)
        {
            b.transform.parent = _opponentArmyHolder;
            EnemyUnits.Add(b);
            b.OnDeath += EnemyUnitDies;
            OnEnemyUnitAdded?.Invoke(b);
        }

        void PlayerUnitDies(UnitController be, Attack attack)
        {
            KilledPlayerUnits.Add(be);
            PlayerUnits.Remove(be);
            OnPlayerUnitDeath?.Invoke(be);

            if (IsTesting && IsFightActive && PlayerUnits.Count == 0)
                DebugEndFight(); // HERE: testing
        }

        void EnemyUnitDies(UnitController be, Attack attack)
        {
            KilledEnemyUnits.Add(be.Unit);
            EnemyUnits.Remove(be);
            OnEnemyUnitDeath?.Invoke(be);

            if (IsFightActive && EnemyUnits.Count == 0)
                EndFight();
        }

        void DebugEndFight()
        {
            IsFightActive = false;
            OnFightEnded?.Invoke();
        }

        void EndFight()
        {
            FightNumber++;

            CurrentFight.GiveReward();
            LastFight = CurrentFight;

            CurrentFight = _arena.CreateFight(_heroController.Hero.GetHeroPoints());
            CurrentFight.OnOptionChosen += SpawnEnemyArmy;
            UpdateFightInfoText();

            IsFightActive = false;

            OnFightEnded?.Invoke();

            if (!_battle.FightSelector.IsUnlocked) ChooseRandomFightOption();
        }

        void ChooseRandomFightOption()
        {
            if (IsTesting) return;

            CurrentFight.ChooseRandomOption();
            _tooltipManager.DisplayGameInfo(new Label("Interact with fight starter to start the next fight."));
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

        void UpdateFightInfoText()
        {
            _fightInfoLabel.text = $"Fight {FightNumber} | Hero Points: {_heroController.Hero.GetHeroPoints()}";
        }
    }
}