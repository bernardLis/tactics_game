using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    using Ability;

    public class RewardAbility : Reward
    {
        public bool IsUpgrade { get; private set; }
        public int Level { get; private set; }

        public Ability Ability { get; private set; }

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);

            Ability = GetValidAbility(otherRewardCards);
            if (Ability == null) return false;

            Ability.InitializeBattle(hero);

            foreach (Ability heroAbility in Hero.GetAllAbilities())
            {
                if (heroAbility.Id == Ability.Id)
                {
                    IsUpgrade = true;
                    Level = heroAbility.Level + 2;
                }
            }

            return true;
        }

        Ability GetValidAbility(List<RewardElement> otherRewardCards)
        {
            List<Ability> validAbilities = new(GameManager.UnitDatabase.GetAllBasicAbilities());
            if (Hero.Abilities.Count >= 4) // HERE: ability limit
            {
                validAbilities = new();
                foreach (Ability a in Hero.Abilities)
                    validAbilities.Add(Instantiate(a));
            }

            // advanced abilities
            foreach (Ability a in Hero.AdvancedAbilities)
            {
                if (a.IsMaxLevel()) continue;
                validAbilities.Add(Instantiate(a));
            }

            // remove abilities that are already in the reward pool
            foreach (RewardElement rc in otherRewardCards)
            {
                if (rc is not RewardElementAbility) continue;
                RewardAbility ra = (RewardAbility)rc.Reward;
                validAbilities.RemoveAll(a => a.Id == ra.Ability.Id);
            }

            // remove abilities that are max level
            for (int i = validAbilities.Count - 1; i >= 0; i--)
            {
                Ability a = Hero.GetAbilityById(validAbilities[i].Id);
                if (a == null) continue;
                if (!a.IsMaxLevel()) continue;
                validAbilities.Remove(validAbilities[i]);
            }

            if (validAbilities.Count != 0) return validAbilities[Random.Range(0, validAbilities.Count)];

            // // TODO: possibly error if no abilities to choose from
            Debug.LogError("Reward - no abilities to choose from");
            return null;
        }

        public override void GetReward()
        {
            base.GetReward();
            if (IsUpgrade)
            {
                foreach (Ability heroAbility in Hero.GetAllAbilities())
                    if (heroAbility.Id == Ability.Id)
                        heroAbility.LevelUp();
                return;
            }

            Hero.AddAbility(Ability);
        }
    }
}