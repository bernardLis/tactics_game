using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Ability")]
public class RewardAbility : Reward
{
    public bool IsUpgrade { get; private set; }

    public Ability Ability { get; private set; }

    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        // I only have 4 buttons for abilities, so I can't have more than 4 abilities

        if (_hero.Abilities.Count == 4)
        {
            IsUpgrade = true;
            Ability = _hero.Abilities[Random.Range(0, 4)];
            return;
        }

        float v = Random.value;
        if (v < 0.5f)
        {
            IsUpgrade = true;
            Ability = _hero.Abilities[Random.Range(0, _hero.Abilities.Count)];
            return;
        }

        // choose an ability that hero does not have yet
        List<Ability> abilities = new(_gameManager.HeroDatabase.GetAllAbilities());
        foreach (Ability newAbility in abilities)
            foreach (Ability heroAbility in _hero.Abilities)
                if (newAbility.Id == heroAbility.Id)
                    abilities.Remove(newAbility);

        Ability = abilities[Random.Range(0, abilities.Count)];
    }

    public override void GetReward()
    {
        base.GetReward();
        if (IsUpgrade)
        {
            foreach (Ability heroAbility in _hero.Abilities)
                if (heroAbility.Id == Ability.Id)
                    heroAbility.Upgrade();
            return;
        }

        _hero.AddAbility(Ability);
    }
}
