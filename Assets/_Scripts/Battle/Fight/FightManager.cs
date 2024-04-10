using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Units;
using Lis.Units.Creature;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Fight
{
    public class FightManager : MonoBehaviour
    {
        public Transform EntityHolder;

        [HideInInspector] public Fight CurrentFight;

        Label _timerLabel;
        VisualElement _debugInfoContainer;
        Label _debugInfoLabel;

        public List<UnitController> PlayerUnits = new();
        public List<UnitController> OpponentUnits = new();

        public List<UnitController> KilledPlayerUnits = new();
        public List<Unit> KilledOpponentUnits = new();

        public event Action<UnitController> OnPlayerUnitAdded;
        public event Action<UnitController> OnPlayerUnitDeath;
        public event Action<UnitController> OnOpponentUnitAdded;
        public event Action<UnitController> OnOpponentUnitDeath;

        IEnumerator _fightCoroutine;
        int _fightTime;

        public void Initialize()
        {
            SetupUi();
            CreateFight();
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
            Fight fight = ScriptableObject.CreateInstance<Fight>();
            fight.CreateFight();
            CurrentFight = fight;
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
            // TODO: Start fight

            yield return new WaitForSeconds(2f);
            UpdateDebugInfo();
        }

        public void AddPlayerUnit(UnitController b)
        {
            b.transform.parent = EntityHolder;
            PlayerUnits.Add(b);
            b.OnDeath += PlayerUnitDies;
            if (b is CreatureController creature)
                OnPlayerUnitAdded?.Invoke(creature);
        }

        public void AddOpponentUnit(UnitController b)
        {
            OpponentUnits.Add(b);
            b.OnDeath += OpponentUnitDies;
            OnOpponentUnitAdded?.Invoke(b);
        }

        void PlayerUnitDies(UnitController be, UnitController killer)
        {
            KilledPlayerUnits.Add(be);
            PlayerUnits.Remove(be);
            OnPlayerUnitDeath?.Invoke(be);
        }

        void OpponentUnitDies(UnitController be, UnitController killer)
        {
            KilledOpponentUnits.Add(be.Unit);
            OpponentUnits.Remove(be);
            OnOpponentUnitDeath?.Invoke(be);
        }

        public List<UnitController> GetAllies(UnitController unitController)
        {
            if (unitController.Team == 0) return PlayerUnits;
            return OpponentUnits;
        }

        public List<UnitController> GetOpponents(int team)
        {
            if (team == 0) return OpponentUnits;
            return PlayerUnits;
        }

        public Vector3 GetRandomEnemyPosition()
        {
            if (OpponentUnits.Count == 0) return Vector3.zero;
            return OpponentUnits[UnityEngine.Random.Range(0, OpponentUnits.Count)].transform.position;
        }

        void UpdateDebugInfo()
        {
            // TODO: Update debug info
            // _debugInfoLabel.text =
            //     "Wave: " + wave.WaveIndex + " Points: " + wave.Points + " Level: " + wave.MinionLevel;
        }
    }
}