using System.Collections;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Battle.Tiles;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle
{
    public class BattleInitializer : MonoBehaviour
    {
        AudioManager _audioManager;
        GameManager _gameManager;
        BattleManager _battleManager;

        LoadingScreen _loadingScreen;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _battleManager = BattleManager.Instance;
            _audioManager = AudioManager.Instance;
            _audioManager.MuteAllButMusic();

            _loadingScreen = new LoadingScreen();
            _battleManager.Initialize();
            _battleManager.ResumeGame();

            StartCoroutine(DelayedStart());
        }

        IEnumerator DelayedStart()
        {

            yield return new WaitForSeconds(0.5f);
            GetComponent<AreaManager>().Initialize();

            HeroManager heroManager = GetComponent<HeroManager>();
            heroManager.enabled = true;
            heroManager.Initialize(_gameManager.SelectedHero);

            yield return new WaitForSeconds(0.5f);

            GetComponent<GrabManager>().Initialize();
            GetComponent<AreaManager>().SecureHomeTile();

            yield return new WaitForSeconds(0.1f);
            GetComponent<TooltipManager>().Initialize();
            GetComponent<FightManager>().Initialize();
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
            yield return new WaitForSeconds(0.1f);


            _loadingScreen.Hide();
            _audioManager.UnmuteAll();
        }
    }
}