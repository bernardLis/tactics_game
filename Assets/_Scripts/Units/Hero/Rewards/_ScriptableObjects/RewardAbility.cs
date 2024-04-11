using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    public class RewardAbility : Reward
    {
        public bool IsUpgrade { get; private set; }
        public int Level { get; private set; }

        public Ability.Ability Ability { get; private set; }

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);

            Ability = GetValidAbility(otherRewardCards);
            if (Ability == null) return false;
            Ability.Level = 0;

            foreach (Ability.Ability heroAbility in Hero.GetAllAbilities())
            {
                if (heroAbility.Id == Ability.Id)
                {
                    IsUpgrade = true;
                    Level = heroAbility.Level + 2;
                    Ability.Level = heroAbility.Level + 1;
                }
            }

            Ability.InitializeBattle(hero);

            return true;
        }

        Ability.Ability GetValidAbility(List<RewardElement> otherRewardCards)
        {
            List<Ability.Ability> validAbilities = new();
            foreach (Ability.Ability a in GameManager.UnitDatabase.GetAllBasicAbilities())
                validAbilities.Add(Instantiate(a));

            if (Hero.Abilities.Count >= 4) // HERE: ability limit
            {
                validAbilities = new();
                foreach (Ability.Ability a in Hero.Abilities)
                    validAbilities.Add(Instantiate(a));
            }

            // advanced abilities
            foreach (Ability.Ability a in Hero.AdvancedAbilities)
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
                Ability.Ability a = Hero.GetAbilityById(validAbilities[i].Id);
                if (a == null) continue;
                if (!a.IsMaxLevel()) continue;
                validAbilities.Remove(validAbilities[i]);
            }

            if (validAbilities.Count != 0) return validAbilities[Random.Range(0, validAbilities.Count)];

            // // TODO: possibly error if no abilities to choose from
            Debug.LogWarning("Reward - no abilities to choose from");
            return null;
        }

        public override void GetReward()
        {
            base.GetReward();
            if (IsUpgrade)
            {
                foreach (Ability.Ability heroAbility in Hero.GetAllAbilities())
                    if (heroAbility.Id == Ability.Id)
                        heroAbility.LevelUp();
                return;
            }

            Hero.AddAbility(Ability);
        }
    }
}