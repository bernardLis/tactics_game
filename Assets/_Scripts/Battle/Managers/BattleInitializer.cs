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

            Hero newChar = ScriptableObject.CreateInstance<Hero>();
            newChar.CreateHero("HERO", _gameManager.EntityDatabase.GetRandomElement());

            Battle battle = ScriptableObject.CreateInstance<Battle>();
            battle.CreateRandom(1);
            _gameManager.CurrentBattle = battle;

            StartCoroutine(DelayedStart(newChar));
        }

        IEnumerator DelayedStart(Hero h)
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<BattleAreaManager>().Initialize();

            yield return new WaitForSeconds(2.5f);

            GetComponent<BattleManager>().Initialize(h);
            GetComponent<BattleGrabManager>().Initialize();

            yield return new WaitForSeconds(1f);

            GetComponent<BattleTooltipManager>().Initialize();
            GetComponent<BattleMinionManager>().Initialize();
            GetComponent<BattleVaseManager>().Initialize();
            GetComponent<BattlePickupManager>().Initialize();
            GetComponent<BattleProjectileManager>().Initialize();
            GetComponent<BattleBossManager>().Initialize();
        }
    }
}