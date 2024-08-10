using System;
using System.Collections;
using Lis.Arena.Pickup;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class ArenaInitializer : Singleton<ArenaInitializer>
    {
        [SerializeField] ThemeStyleSheet[] _themeStyleSheets;
        [SerializeField] Sound _music;

        AudioManager _audioManager;
        GameManager _gameManager;

        FightManager _fightManager;
        LoadingScreen _loadingScreen;

        public event Action OnArenaInitialized;

        void Start()
        {
            _gameManager = GameManager.Instance;

            // HERE: testing
            _audioManager = AudioManager.Instance;
            _audioManager.MuteAllButMusic();

            _loadingScreen = new();
            _fightManager = FightManager.Instance;
            _fightManager.Initialize(_gameManager.Campaign);
            Time.timeScale = 1;

            StartCoroutine(DelayedStart());
        }


        IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(_gameManager.Campaign.CurrentArena.Prefab, Vector3.zero, Quaternion.identity);
            ArenaManager.Instance.Initialize();
            yield return new WaitForSeconds(0.5f);

            HeroManager heroManager = GetComponent<HeroManager>();
            heroManager.enabled = true;
            heroManager.Initialize(_gameManager.Campaign.Hero);
            SetThemeStyleSheet();

            yield return new WaitForSeconds(0.5f);
            GetComponent<GrabManager>().Initialize();
            GetComponent<TooltipManager>().Initialize();
            GetComponent<EnemyPoolManager>().Initialize();
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
            yield return new WaitForSeconds(0.1f);

            OnArenaInitialized?.Invoke();

            // HERE: testing
            _loadingScreen.Hide();
            _audioManager.UnmuteAll();
            _audioManager.PlayMusic(_music);
        }

        void SetThemeStyleSheet()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            foreach (ThemeStyleSheet themeStyleSheet in _themeStyleSheets)
            {
                if (themeStyleSheet.Nature ==
                    _gameManager.UnitDatabase.GetRandomNature())
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
    internal struct ThemeStyleSheet
    {
        public Nature Nature;
        public StyleSheet StyleSheet;
    }
}