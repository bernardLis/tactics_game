using System;
using System.Collections;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class BattleManager : Singleton<BattleManager>
    {
        public static bool BlockBattleInput;

        GameManager _gameManager;
        AudioManager _audioManager;

        [SerializeField] Sound _battleLost;
        [SerializeField] Sound _battleWon;

        public Battle Battle;

        public VisualElement Root { get; private set; }

        public Transform EntityHolder;
        public Transform AbilityHolder;
        public Transform ProjectilePoolHolder;

        public bool IsTimerOn { get; private set; }
        bool _battleFinalized;

        IEnumerator _timerCoroutine;
        int _battleTime;
        Label _timerLabel;


        public event Action OnBattleInitialized;
        public event Action OnBattleFinalized;

        public event Action OnGamePaused;
        public event Action OnGameResumed;

        protected override void Awake()
        {
            base.Awake();

            Root = GetComponent<UIDocument>().rootVisualElement;
        }

        void Start()
        {
            // HERE: render texture issue unity must resolve
            // VFXCameraManager.Instance.gameObject.SetActive(false);
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _gameManager.SaveJsonData();

            Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

            _timerLabel = Root.Q<Label>("timer");
        }

        public void Initialize()
        {
            Battle = _gameManager.CurrentBattle;

            _battleFinalized = false;
            _battleTime = 0;
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
                if (this == null) yield break;
                _battleTime++;
                int minutes = Mathf.FloorToInt(_battleTime / 60f);
                int seconds = Mathf.FloorToInt(_battleTime - minutes * 60);

                _timerLabel.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(1f);
            }
        }

        public float GetTime()
        {
            return _battleTime;
        }

        IEnumerator _endBattleCoroutine;

        public void LoseBattle()
        {
            if (_endBattleCoroutine != null) return;
            _endBattleCoroutine = BattleLost();
            StartCoroutine(_endBattleCoroutine);
        }

        public void WinBattle()
        {
            if (_endBattleCoroutine != null) return;
            _endBattleCoroutine = BattleWon();
            StartCoroutine(_endBattleCoroutine);
        }

        public void SkullCollected()
        {
            if (this == null) return;
            // TODO: skull collected
        }

        IEnumerator BattleLost()
        {
            _audioManager.PlayUI(_battleLost);
            LostBattleScreen lostScreen = new();
            yield return FinalizeBattle();
        }

        IEnumerator BattleWon()
        {
            _audioManager.PlayUI(_battleWon);
            WonBattleScreen wonScreen = new();
            yield return FinalizeBattle();
        }

        IEnumerator FinalizeBattle()
        {
            // if entities die "at the same time" it triggers twice
            if (_battleFinalized) yield break;
            _battleFinalized = true;
            _audioManager.BattleSfxCleanup();
            _gameManager.GameStats.AddStats(Battle.Stats);

            yield return new WaitForSeconds(3f);

            OnBattleFinalized?.Invoke();
        }

#if UNITY_EDITOR

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
#endif
    }
}