using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleInitializer : MonoBehaviour
    {
        GameManager _gameManager;
        
        void Start()
        {
            _gameManager = GameManager.Instance;

            Hero h = Instantiate(_gameManager.SelectedHero);
            h.InitializeHero();

            Battle battle = ScriptableObject.CreateInstance<Battle>();
            battle.CreateRandom(1);
            _gameManager.CurrentBattle = battle;

            StartCoroutine(DelayedStart(h));
        }

        IEnumerator DelayedStart(Hero h)
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<BattleAreaManager>().Initialize();
            yield return new WaitForSeconds(1f);

            GetComponent<BattleManager>().Initialize(h);
            GetComponent<BattleGrabManager>().Initialize();
            GetComponent<BattleAreaManager>().SecureHomeTile();
            
            yield return new WaitForSeconds(2f);
            GetComponent<BattleTooltipManager>().Initialize();
            GetComponent<BattleFightManager>().Initialize();
            GetComponent<BattleVaseManager>().Initialize();
            GetComponent<BattlePickupManager>().Initialize();
            GetComponent<BattleRangedOpponentManager>().Initialize();
            GetComponent<BattleBossManager>().Initialize();
            GetComponent<BattleStatsTracker>().Initialize();
        }
    }
}