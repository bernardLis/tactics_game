using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

#if (UNITY_EDITOR) 

public class BattleEndTestManager : MonoBehaviour
{
    BattleManager _battleManager;
    Hero _hero;

    [SerializeField] List<ArmyGroup> _army;

    [SerializeField] List<Pickup> _pickups;

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
        GameManager.Instance.PlayerHero = _hero;

        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }

        Hero opp = ScriptableObject.CreateInstance<Hero>();
        opp.CreateRandom(1);
        opp.Army = armyInstance;

        _battleManager.Initialize(_hero, opp, _hero.Army, opp.Army);

        for (int i = 0; i < 10; i++)
        {
            List<Pickup> ordered = new(_pickups.OrderBy(o => o.PickupChance).ToList());
            float roll = Random.value;

            foreach (Pickup p in ordered)
            {
                if (roll <= p.PickupChance)
                {
                    Pickup instance = Instantiate(p);
                    instance.Initialize();
                    _battleManager.CollectPickup(instance);
                    break;
                }
                roll -= p.PickupChance;
            }
        }

        Invoke("WinBattle", 1f);
    }

    void WinBattle()
    {
        _battleManager.WinBattle();
    }
}
#endif
