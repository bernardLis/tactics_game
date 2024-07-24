using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Attack;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Arena.Fight
{
    public class FightManager : Singleton<FightManager>
    {
        public static bool IsFightActive { get; private set; }
        public static bool BlockPlayerInput { get; private set; }

        [SerializeField] Sound _arenaLost;
        [SerializeField] Sound _arenaWon;

        public Transform PlayerArmyHolder;
        public Transform EnemyArmyHolder;
        public Transform EffectHolder;
        public Transform AbilityHolder;
        public Transform ProjectilePoolHolder;

        public List<UnitController> PlayerUnits = new();
        public List<UnitController> EnemyUnits = new();

        public List<UnitController> KilledPlayerUnits = new();
        public List<Unit> KilledEnemyUnits = new();
        public bool IsTesting;

        public VisualElement Root { get; private set; }
        Label _fightInfoLabel;
        Label _timerLabel;

        public Campaign Campaign;

        Arena _arena;
        [HideInInspector] public Wave CurrentWave;
        int _currentWaveIndex;
        bool _arenaFinalized;

        GameManager _gameManager;
        AudioManager _audioManager;
        ArenaManager _arenaManager;
        EnemyPoolManager _enemyPoolManager;
        HeroController _heroController;
        TooltipManager _tooltipManager;

        IEnumerator _fightCoroutine;
        IEnumerator _endFightCoroutine;

        int _fightTime;
        float _lastTimeScale = 1f;
        IEnumerator _timerCoroutine;
        public bool IsTimerOn { get; private set; }

        public event Action OnGamePaused;
        public event Action OnGameResumed;
        public event Action<UnitController> OnEnemyUnitDeath;
        public event Action OnFightStarted;
        public event Action OnFightEnded;
        public event Action OnFightFinalized;

        protected override void Awake()
        {
            base.Awake();
            Root = GetComponent<UIDocument>().rootVisualElement;
        }

        public void Initialize(Campaign campaign)
        {
            Campaign = campaign;
            _arena = Campaign.CurrentArena;

            _arenaManager = GetComponent<ArenaManager>();
            _enemyPoolManager = GetComponent<EnemyPoolManager>();
            _tooltipManager = GetComponent<TooltipManager>();

            _timerLabel = Root.Q<Label>("timer");

            ArenaInitializer.Instance.OnArenaInitialized += StartGame;
        }

        void StartGame()
        {
            _heroController = GetComponent<HeroManager>().HeroController;

            CurrentWave = _arena.Initialize(_heroController.Hero);

            StartCoroutine(SpawnAllPlayerUnits());

            IsFightActive = false;

            _arenaFinalized = false;
            _fightTime = 0;

            ResumeTimer();

            StartFight();
        }

        void StartFight()
        {
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

            for (int i = 0; i < 3; i++)
            {
                _tooltipManager.DisplayGameInfo(new Label((3 - i).ToString()));
                yield return new WaitForSeconds(1f);
            }

            _currentWaveIndex = 0;
            StartCoroutine(SpawnEnemyWavesCoroutine());

            _heroController.StartAllAbilities();
            OnFightStarted?.Invoke();
        }

        IEnumerator SpawnEnemyWavesCoroutine()
        {
            for (int i = 0; i < _arena.Waves.Count; i++)
            {
                _tooltipManager.DisplayGameInfo(
                    new Label($"Wave {_currentWaveIndex + 1}/{_arena.Waves.Count}"));

                SpawnEnemyArmy(CurrentWave);

                _currentWaveIndex++;
                if (_currentWaveIndex == _arena.Waves.Count) yield break;
                CurrentWave = _arena.Waves[_currentWaveIndex];

                yield return new WaitForSeconds(Random.Range(10, 15));
            }
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

        void SpawnEnemyArmy(Wave wave)
        {
            StartCoroutine(SpawnEnemyUnits(wave.Army));
        }

        IEnumerator SpawnEnemyUnits(Dictionary<string, int> army)
        {
            foreach (KeyValuePair<string, int> keyValue in army)
            {
                for (int i = 0; i < keyValue.Value; i++)
                {
                    SpawnEnemyUnit(keyValue.Key);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        public UnitController SpawnEnemyUnit(string id)
        {
            Vector3 pos = _arenaManager.GetRandomPositionInEnemyLockerRoom();
            UnitController unitController = _enemyPoolManager.Get(id);

            unitController.transform.position = pos;
            unitController.SetOpponentList(ref PlayerUnits);
            AddEnemyUnit(unitController);
            return unitController;
        }

        public void AddPlayerUnit(UnitController uc)
        {
            uc.transform.parent = PlayerArmyHolder;
            PlayerUnits.Add(uc);
            uc.OnDeath += PlayerUnitDies;

            if (uc is PlayerUnitController puc)
                puc.OnRevived += () => PlayerUnits.Add(uc);
        }

        public void AddEnemyUnit(UnitController b)
        {
            EnemyUnits.Add(b);
            b.OnDeath += EnemyUnitDies;
        }

        void PlayerUnitDies(UnitController uc, Attack attack)
        {
            KilledPlayerUnits.Add(uc);
            PlayerUnits.Remove(uc);

            if (IsTesting && IsFightActive && PlayerUnits.Count == 0)
                DebugEndFight(); // HERE: testing
        }

        void EnemyUnitDies(UnitController uc, Attack attack)
        {
            KilledEnemyUnits.Add(uc.Unit);
            EnemyUnits.Remove(uc);
            OnEnemyUnitDeath?.Invoke(uc);

            if (!IsFightActive) return;
            if (EnemyUnits.Count > 0) return;
            if (_currentWaveIndex != _arena.Waves.Count) return;
            EndFight();
        }

        void DebugEndFight()
        {
            IsFightActive = false;
            OnFightEnded?.Invoke();
        }

        void EndFight()
        {
            IsFightActive = false;

            _arenaManager.ChooseActiveEnemyLockerRooms(CurrentWave.ActiveLockerRoomCount);

            OnFightEnded?.Invoke();
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

        /* TIMER */

        public void PauseGame()
        {
            Debug.Log("Pausing the game...");
            if (Time.timeScale != 0)
                _lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            PauseTimer();
            BlockPlayerInput = true;

            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            Debug.Log("Resuming the game...");
            Time.timeScale = _lastTimeScale;
            ResumeTimer();
            BlockPlayerInput = false;

            OnGameResumed?.Invoke();
        }

        void PauseTimer()
        {
            if (this == null) return;

            IsTimerOn = false;
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);
        }

        void ResumeTimer()
        {
            IsTimerOn = true;
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = UpdateTimer();
            StartCoroutine(_timerCoroutine);
        }

        IEnumerator UpdateTimer()
        {
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

        public float GetTime()
        {
            return _fightTime;
        }

        public void LoseArena()
        {
            if (_endFightCoroutine != null) return;
            _endFightCoroutine = FightLostCoroutine();
            StartCoroutine(_endFightCoroutine);
        }

        public void WinArena()
        {
            if (_endFightCoroutine != null) return;
            _endFightCoroutine = FightWonCoroutine();
            StartCoroutine(_endFightCoroutine);
        }

        public void SkullCollected()
        {
            if (this == null) return;
            // TODO: skull collected
        }

        IEnumerator FightLostCoroutine()
        {
            _audioManager.CreateSound().WithSound(_arenaLost).Play();
            LostArenaScreen lostScreen = new();
            yield return FinalizeArenaCoroutine();
        }

        IEnumerator FightWonCoroutine()
        {
            _audioManager.CreateSound().WithSound(_arenaWon).Play();
            WonArenaScreen wonScreen = new();
            yield return FinalizeArenaCoroutine();
        }

        IEnumerator FinalizeArenaCoroutine()
        {
            // if entities die "at the same time" it triggers twice
            if (_arenaFinalized) yield break;
            _arenaFinalized = true;
            _audioManager.ArenaSfxCleanup();
            _gameManager.GameStats.AddStats(Campaign.Stats);

            yield return new WaitForSeconds(3f);

            OnFightFinalized?.Invoke();
        }


#if UNITY_EDITOR

        [ContextMenu("Force Win Arena")]
        public void WinArenaAlternative()
        {
            StartCoroutine(FightWonCoroutine());
        }

        [ContextMenu("Force Lose Arena")]
        public void LoseArenaAlternative()
        {
            StartCoroutine(FightLostCoroutine());
        }
#endif
    }
}