using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Ability")]
public class RewardAbility : Reward
{
    public bool IsUpgrade { get; private set; }

    public Ability Ability { get; private set; }

    public override bool CreateRandom(Hero hero, List<RewardCard> otherRewardCards)
    {
        base.CreateRandom(hero, otherRewardCards);

        List<Ability> abilitiesAlreadyInRewardPool = new();
        foreach (RewardCard rc in otherRewardCards)
        {
            if (rc is not RewardCardAbility) continue;
            RewardAbility ra = (RewardAbility)rc.Reward;
            abilitiesAlreadyInRewardPool.Add(ra.Ability);
        }

        Ability = GetValidAbility(abilitiesAlreadyInRewardPool);
        if (Ability == null) return false;
        
        Ability.InitializeBattle();

        foreach (Ability heroAbility in _hero.Abilities)
            if (heroAbility.Id == Ability.Id)
                IsUpgrade = true;

        return true;
    }

    public Ability GetValidAbility(List<Ability> forbiddenAbilities)
    {
        List<Ability> abilities = new(_gameManager.EntityDatabase.GetAllAbilities());
        if (_hero.Abilities.Count == 4) // only 4 ability buttons // HERE: ability limit
            abilities = new(_hero.Abilities);

        abilities.RemoveAll(x => forbiddenAbilities.Contains(x));

        for (int i = abilities.Count - 1; i >= 0; i--)
            if (!abilities[i].HasMoreUpgrades())
                abilities.Remove(abilities[i]);

        // TODO: possibly error if no abilities to choose from
        if (abilities.Count == 0)
        {
            Debug.LogError("Reward - no abilities to choose from");
            return null;
        }

        return abilities[Random.Range(0, abilities.Count)];
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
