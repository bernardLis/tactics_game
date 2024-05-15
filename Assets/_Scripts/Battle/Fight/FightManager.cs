using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Attack;
using Lis.Units.Creature;
using Lis.Units.Hero;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    using Arena;

    public class FightManager : Singleton<FightManager>
    {
        ArenaManager _arenaManager;
        public Transform PlayerArmyHolder;
        [SerializeField] Transform _opponentArmyHolder;

        Battle _battle;
        Arena _arena;

        [HideInInspector] public Fight CurrentFight;

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

        public static bool IsFightActive { get; private set; }
        IEnumerator _fightCoroutine;

        public void Initialize(Battle battle)
        {
            _battle = battle;
            _arena = _battle.CurrentArena;

            _arenaManager = GetComponent<ArenaManager>();
            _heroController = GetComponent<HeroManager>().HeroController;

            CurrentFight = _arena.CreateFight();
            StartCoroutine(SpawnAllPlayerUnits());
            StartCoroutine(SpawnEnemyUnits());

            // HERE: testing
            GetComponent<InputManager>().OnOneClicked += StartFight;
        }

        public void StartFight()
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

        IEnumerator SpawnEnemyUnits()
        {
            foreach (Creature c in _arena.Fights.Last().OpponentArmy)
            {
                SpawnEnemyUnit(c);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void SpawnEnemyUnit(Creature c)
        {
            c.InitializeBattle(1);
            Vector3 pos = _arenaManager.GetRandomPositionInEnemyLockerRoom();
            GameObject g = Instantiate(c.Prefab, pos, Quaternion.identity, PlayerArmyHolder);
            UnitController unitController = g.GetComponent<UnitController>();
            unitController.InitializeGameObject();
            unitController.InitializeUnit(c, 1);
            unitController.SetOpponentList(ref PlayerUnits);
            AddEnemyUnit(unitController);
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
        }

        void EnemyUnitDies(UnitController be, Attack attack)
        {
            KilledEnemyUnits.Add(be.Unit);
            EnemyUnits.Remove(be);
            OnEnemyUnitDeath?.Invoke(be);

            if (IsFightActive && EnemyUnits.Count == 0)
                EndFight();
        }

        void EndFight()
        {
            CurrentFight = _arena.CreateFight();
            IsFightActive = false;
            OnFightEnded?.Invoke();

            StartCoroutine(EndFightCoroutine());
        }

        IEnumerator EndFightCoroutine()
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(SpawnEnemyUnits());
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
    }
}