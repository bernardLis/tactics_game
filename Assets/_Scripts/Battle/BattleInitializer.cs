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

        LoadingScreen _loadingScreen;

        void Awake()
        {
            _loadingScreen = new LoadingScreen();
        }

        void Start()
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _audioManager.MuteAllButMusic();

            Battle battle = ScriptableObject.CreateInstance<Battle>();
            battle.Initialize(1);
            _gameManager.CurrentBattle = battle;

            BattleManager.Instance.ResumeGame();

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

            yield return new WaitForSeconds(0.2f);
            GetComponent<TooltipManager>().Initialize();
            GetComponent<FightManager>().Initialize();
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
            yield return new WaitForSeconds(0.2f);
            GetComponent<BattleManager>().Initialize();


            _loadingScreen.Hide();
            _audioManager.UnmuteAll();
        }
    }
}