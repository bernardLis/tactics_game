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

        List<ArmyGroup> armyInstance = new();
        foreach (ArmyGroup ag in _army)
        {
            ArmyGroup instance = Instantiate<ArmyGroup>(ag);
            armyInstance.Add(instance);
        }

        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.Army = armyInstance;
        // HERE: cutscene testing
        GameManager.Instance.PlayerHero = _hero;
        GameManager.Instance.OverseerHero = ScriptableObject.CreateInstance<Hero>();
        GameManager.Instance.OverseerHero.CreateRandom(99);

        GameManager.Instance.RivalHero = ScriptableObject.CreateInstance<Hero>();
        GameManager.Instance.RivalHero.CreateRandom(3);

        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }

        Hero opp = ScriptableObject.CreateInstance<Hero>();
        opp.CreateRandom(1);
        opp.Army = armyInstance;

        _battleManager.Initialize(_hero, opp, _hero.Army, opp.Army);

        Invoke("WinBattle", 1f);
    }

    void WinBattle()
    {
        _battleManager.WinBattle();
    }
}
