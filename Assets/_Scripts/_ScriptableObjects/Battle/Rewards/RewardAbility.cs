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

        List<Ability> abilities = new(_gameManager.HeroDatabase.GetAllAbilities());
        if (_hero.Abilities.Count == 4) // only 4 ability buttons
            abilities = new(_hero.Abilities);
        Ability = abilities[Random.Range(0, abilities.Count)];
        
        // Tutorial
        if (hero.Level.Value == 2)
            Ability = _gameManager.HeroDatabase.GetAbilityById("fireball-7f5d-4f59-86f9-bb8f8676274d");

        foreach (Ability heroAbility in _hero.Abilities)
            if (heroAbility.Id == Ability.Id)
                IsUpgrade = true;
    }

    public override void GetReward()
    {
        base.GetReward();
        if (IsUpgrade)
        {
            foreach (Ability heroAbility in _hero.Abilities)
                if (heroAbility.Id == Ability.Id)
                    heroAbility.LevelUp();
            return;
        }

        _hero.AddAbility(Ability);
    }
}
