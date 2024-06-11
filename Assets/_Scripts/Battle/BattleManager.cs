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

        [SerializeField] Sound _battleLost;
        [SerializeField] Sound _battleWon;

        public Battle Battle;

        public Transform EntityHolder;
        public Transform AbilityHolder;
        public Transform ProjectilePoolHolder;
        AudioManager _audioManager;
        bool _battleFinalized;
        int _battleTime;

        IEnumerator _endBattleCoroutine;

        GameManager _gameManager;

        float _lastTimeScale = 1f;

        IEnumerator _timerCoroutine;
        Label _timerLabel;

        public VisualElement Root { get; private set; }

        public bool IsTimerOn { get; private set; }

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

        public event Action OnBattleFinalized;

        public event Action OnGamePaused;
        public event Action OnGameResumed;

        public void Initialize()
        {
            Battle = _gameManager.CurrentBattle;

            _battleFinalized = false;
            _battleTime = 0;
            ResumeTimer();
        }

        public void PauseGame()
        {
            Debug.Log("Pausing the game...");
            if (Time.timeScale != 0)
                _lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            PauseTimer();
            BlockBattleInput = true;

            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            Debug.Log("Resuming the game...");
            Time.timeScale = _lastTimeScale;
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