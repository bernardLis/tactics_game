using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEndTestManager : MonoBehaviour
{
    BattleManager _battleManager;
    Hero _hero;

    void Start()
    {
        _battleManager = BattleManager.Instance;

        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.BaseMana.SetValue(12);

        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }

        _battleManager.Initialize(_hero, null, null, null);
        GameManager.Instance.ToggleTimer(true);

    }
}
