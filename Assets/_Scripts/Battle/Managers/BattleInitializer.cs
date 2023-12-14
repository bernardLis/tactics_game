using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class BattleInitializer : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    BattleInputManager _battleInputManager;
    BattleMinionManager _battleMinionManager;
    BattleBossManager _battleBossManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleMinionManager = _battleManager.GetComponent<BattleMinionManager>();
        _battleBossManager = _battleManager.GetComponent<BattleBossManager>();

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
        _battleAreaManager.Initialize();

        yield return new WaitForSeconds(2.5f);

        _battleManager.Initialize(h);
        _battleManager.GetComponent<BattleGrabManager>().Initialize();

        _battleMinionManager.Initialize();
        _battleBossManager.Initialize();
        BattleTooltipManager.Instance.Initialize();
    }
}
