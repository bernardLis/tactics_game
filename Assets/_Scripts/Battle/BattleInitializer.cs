using System.Collections;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle
{
    public class BattleInitializer : MonoBehaviour
    {
        // AudioManager _audioManager;
        GameManager _gameManager;
        BattleManager _battleManager;

        LoadingScreen _loadingScreen;

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

            yield return new WaitForSeconds(0.5f);
            GetComponent<GrabManager>().Initialize();
            GetComponent<TooltipManager>().Initialize();
            GetComponent<FightManager>().Initialize(_gameManager.CurrentBattle);
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
            yield return new WaitForSeconds(0.1f);

            // HERE: testing
            // _loadingScreen.Hide();
            // _audioManager.UnmuteAll();
            GetComponent<FightManager>().StartFight();
        }
    }
}