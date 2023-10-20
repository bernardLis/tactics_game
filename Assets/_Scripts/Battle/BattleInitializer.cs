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
    BattleDeploymentManager _battleDeploymentManager;

    Hero _playerHero;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateRandomHero("asd", _gameManager.EntityDatabase.GetRandomElement());
        _gameManager.PlayerHero = newChar;

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandom(1);
        _gameManager.CurrentBattle = battle;

        StartCoroutine(DelayedStart(newChar));
    }

    IEnumerator DelayedStart(Hero h)
    {
        yield return new WaitForSeconds(0.5f);
        _battleAreaManager.Initialize();

        yield return new WaitForSeconds(0.5f);

        _battleManager.Initialize(h);
        _battleManager.GetComponent<BattleGrabManager>().Initialize();
    }
}
