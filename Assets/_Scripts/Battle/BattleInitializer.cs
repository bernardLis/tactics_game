using System;
using System.Collections;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class BattleInitializer : MonoBehaviour
    {
        // AudioManager _audioManager;
        GameManager _gameManager;
        BattleManager _battleManager;

        LoadingScreen _loadingScreen;

        [SerializeField] ThemeStyleSheet[] _themeStyleSheets;

        public event Action OnBattleInitialized;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _battleManager = BattleManager.Instance;

            // HERE: testing
            // _audioManager = AudioManager.Instance;
            // _audioManager.MuteAllButMusic();

            // _loadingScreen = new();
            _battleManager.Initialize();
            // _battleManager.ResumeGame();

            StartCoroutine(DelayedStart());
        }

        IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<ArenaManager>().Initialize(_gameManager.CurrentBattle);
            yield return new WaitForSeconds(0.5f);

            HeroManager heroManager = GetComponent<HeroManager>();
            heroManager.enabled = true;
            heroManager.Initialize(_gameManager.CurrentBattle.SelectedHero);
            SetThemeStyleSheet();

            yield return new WaitForSeconds(0.5f);
            GetComponent<GrabManager>().Initialize();
            GetComponent<TooltipManager>().Initialize();
            GetComponent<FightManager>().Initialize(_gameManager.CurrentBattle);
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
            GetComponent<BattleUIManager>().Initialize();
            yield return new WaitForSeconds(0.1f);


            OnBattleInitialized?.Invoke();

            // HERE: testing
            // _loadingScreen.Hide();
            // _audioManager.UnmuteAll();
        }

        void SetThemeStyleSheet()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            foreach (ThemeStyleSheet themeStyleSheet in _themeStyleSheets)
            {
                if (themeStyleSheet.Nature == _gameManager.CurrentBattle.SelectedHero.Nature)
                {
                    root.styleSheets.Add(themeStyleSheet.StyleSheet);
                    continue;
                }

                if (root.styleSheets.Contains(themeStyleSheet.StyleSheet))
                    root.styleSheets.Remove(themeStyleSheet.StyleSheet);
            }
        }
    }

    [Serializable]
    struct ThemeStyleSheet
    {
        public Nature Nature;
        public StyleSheet StyleSheet;
    }
}