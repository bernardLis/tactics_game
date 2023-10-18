using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Random = UnityEngine.Random;
#if (UNITY_EDITOR)

public class BattleEntityTestManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation("asd", _gameManager.EntityDatabase.GetRandomElement());
        _gameManager.PlayerHero = newChar;

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandom(1);
        _gameManager.CurrentBattle = battle;

        StartCoroutine(LateInitialize(newChar));
    }

    IEnumerator LateInitialize(Hero h)
    {
        yield return new WaitForSeconds(0.5f);
        _battleAreaManager.Initialize();

        yield return new WaitForSeconds(0.5f);

        _battleManager.Initialize(h);

        _battleManager.GetComponent<BattleGrabManager>().Initialize();
    }
}

#endif
