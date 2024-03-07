using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Minion;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class BattleManager : Singleton<BattleManager>
    {
        public static bool BlockBattleInput;

        GameManager _gameManager;

        HeroManager _heroManager;

        [SerializeField] Sound _battleMusic;

        public bool IsGameLoopBlocked;

        public Battle Battle;

        public VisualElement Root { get; private set; }

        Label _timerLabel;

        public Transform EntityHolder;
        public Transform AbilityHolder;
        public Transform ProjectilePoolHolder;

        public bool IsTimerOn { get; private set; }

        public Hero Hero { get; private set; }
        public HeroController HeroController => _heroManager.HeroController;

        public List<UnitController> PlayerEntities = new();
        public List<UnitController> OpponentEntities = new();

        public List<UnitController> KilledPlayerEntities = new();
        public List<Unit> KilledOpponentEntities = new();

        public int GoldCollected { get; private set; }

        public bool BlockBattleEnd;
        public bool BattleFinalized { get; private set; }

        IEnumerator _timerCoroutine;
        int _battleTime;

        public event Action OnBattleInitialized;
        public event Action<CreatureController> OnPlayerCreatureAdded;
        public event Action<UnitController> OnPlayerEntityDeath;
        public event Action<UnitController> OnOpponentEntityAdded;
        public event Action<UnitController> OnOpponentEntityDeath;
        public event Action OnBattleFinalized;

        public event Action OnGamePaused;
        public event Action OnGameResumed;

        protected override void Awake()
        {
            base.Awake();

            Root = GetComponent<UIDocument>().rootVisualElement;

            _timerLabel = Root.Q<Label>("timer");
        }

        void Start()
        {
            // HERE: render texture issue unity must resolve
            // VFXCameraManager.Instance.gameObject.SetActive(false);

            _gameManager = GameManager.Instance;
            _gameManager.SaveJsonData();
            Battle = _gameManager.CurrentBattle;
            _gameManager.OnGoldChanged += (g) => GoldCollected += g;

            Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

            _timerLabel.style.display = DisplayStyle.Flex;

#if UNITY_EDITOR
            GetComponent<InputManager>().OnEnterClicked += LevelUpHero;
            GetComponent<InputManager>().OnSpaceClicked += KillAllOpponents;
#endif
        }

        public void Initialize(Hero hero)
        {
            AudioManager.Instance.PlayMusic(_battleMusic);

            BattleFinalized = false;
            _battleTime = 0;

            if (hero != null)
            {
                Hero = hero;
                _heroManager = GetComponent<HeroManager>();
                _heroManager.enabled = true;
                _heroManager.Initialize(hero);
            }

            ResumeTimer();

            OnBattleInitialized?.Invoke();
        }

        public void PauseGame()
        {
            Debug.Log($"Pausing the game...");
            Time.timeScale = 0f;
            PauseTimer();
            BlockBattleInput = true;

            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            Debug.Log($"Resuming the game...");
            Time.timeScale = 1f;
            ResumeTimer();
            BlockBattleInput = false;

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
                _battleTime++;
                float timeLeft = Battle.Duration - _battleTime;
                int minutes = Mathf.FloorToInt(timeLeft / 60f);
                int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);

                _timerLabel.text = $"{minutes:00}:{seconds:00}";
                yield return new WaitForSeconds(1f);

                if (timeLeft <= 0) yield break;
            }
        }

        public float GetTime()
        {
            return _battleTime;
        }

        public float GetTimeLeft()
        {
            return Battle.Duration - _battleTime;
        }

        public void AddPlayerArmyEntity(UnitController b)
        {
            b.transform.parent = EntityHolder;
            PlayerEntities.Add(b);
            b.OnDeath += OnPlayerCreatureDeath;
            if (b is CreatureController creature)
                OnPlayerCreatureAdded?.Invoke(creature);
        }

        public void AddOpponentArmyEntity(UnitController b)
        {
            OpponentEntities.Add(b);
            b.OnDeath += OnOpponentDeath;
            OnOpponentEntityAdded?.Invoke(b);
        }

        void OnPlayerCreatureDeath(UnitController be, UnitController killer)
        {
            KilledPlayerEntities.Add(be);
            PlayerEntities.Remove(be);
            OnPlayerEntityDeath?.Invoke(be);
        }

        void OnOpponentDeath(UnitController be, UnitController killer)
        {
            KilledOpponentEntities.Add(be.Unit);
            OpponentEntities.Remove(be);
            OnOpponentEntityDeath?.Invoke(be);
        }

        public List<UnitController> GetAllies(UnitController unitController)
        {
            if (unitController.Team == 0) return PlayerEntities;
            return OpponentEntities;
        }

        public List<UnitController> GetOpponents(int team)
        {
            if (team == 0) return OpponentEntities;
            return PlayerEntities;
        }

        public Vector3 GetRandomEnemyPosition()
        {
            if (OpponentEntities.Count == 0) return Vector3.zero;
            return OpponentEntities[UnityEngine.Random.Range(0, OpponentEntities.Count)].transform.position;
        }

        public void LoseBattle()
        {
            StartCoroutine(BattleLost());
        }

        public void WinBattle()
        {
            StartCoroutine(BattleWon());
        }

        public void SkullCollected()
        {
            if (this == null) return;
            List<UnitController> copy = new(OpponentEntities);
            foreach (UnitController be in copy)
                if (be is MinionController)
                    StartCoroutine(be.DieCoroutine());
        }

        IEnumerator BattleLost()
        {
            LostBattleScreen lostScreen = new();
            yield return FinalizeBattle();
        }

        IEnumerator BattleWon()
        {
            WonBattleScreen wonScreen = new();
            wonScreen.OnFinishedPlaying += () => StartCoroutine(FinalizeBattle());
            yield return null;
        }

        IEnumerator FinalizeBattle()
        {
            // if entities die "at the same time" it triggers twice
            if (BattleFinalized) yield break;
            BattleFinalized = true;

            yield return new WaitForSeconds(3f);

            // ClearAllEntities();

            OnBattleFinalized?.Invoke();
        }

        public void ClearAllEntities()
        {
            PlayerEntities.Clear();
            OpponentEntities.Clear();
            foreach (Transform child in EntityHolder.transform)
            {
                child.transform.DOKill(child.transform);
                Destroy(child.gameObject);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Kill All Opponents")]
        public void KillAllOpponents()
        {
            if (this == null) return;
            List<UnitController> copy = new(OpponentEntities);
            foreach (UnitController be in copy)
            {
                StartCoroutine(be.DieCoroutine());
            }
        }

        [ContextMenu("Force Win Battle")]
        public void WinBattleAlternative()
        {
            StartCoroutine(BattleWon());
        }

        [ContextMenu("Force Lose Battle")]
        public void LoseBattleAlternative()
        {
            StartCoroutine(BattleLost());
        }

        [ContextMenu("Level up hero")]
        public void LevelUpHero()
        {
            Hero.AddExp(Hero.ExpForNextLevel.Value);
        }

#endif
    }
}