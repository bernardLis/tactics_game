using System.Collections;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Battle.Tiles;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle
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
            battle.Initialize(1);
            _gameManager.CurrentBattle = battle;

            StartCoroutine(DelayedStart(h));
        }

        IEnumerator DelayedStart(Hero h)
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<AreaManager>().Initialize();
            yield return new WaitForSeconds(1f);

            GetComponent<BattleManager>().Initialize(h);
            GetComponent<GrabManager>().Initialize();
            GetComponent<AreaManager>().SecureHomeTile();
            
            yield return new WaitForSeconds(2f);
            GetComponent<TooltipManager>().Initialize();
            GetComponent<FightManager>().Initialize();
            GetComponent<BreakableVaseManager>().Initialize();
            GetComponent<PickupManager>().Initialize();
            GetComponent<RangedOpponentManager>().Initialize();
            GetComponent<BossManager>().Initialize();
            GetComponent<StatsTracker>().Initialize();
        }
    }
}