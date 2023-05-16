using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEndTestManager : MonoBehaviour
{
    BattleManager _battleManager;
    Hero _hero;

    [SerializeField] List<ArmyGroup> _army;

    void Start()
    {
        _battleManager = BattleManager.Instance;

        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.Army = _army;

        GameManager.Instance.PlayerHero = _hero;

        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }

        Hero opp = ScriptableObject.CreateInstance<Hero>();
        opp.CreateRandom(1);
        opp.Army = _army;

        _battleManager.Initialize(_hero, opp, _hero.Army, opp.Army);

        Invoke("WinBattle", 1f);

    }

    void WinBattle()
    {
        _battleManager.WinBattle();
    }
}
