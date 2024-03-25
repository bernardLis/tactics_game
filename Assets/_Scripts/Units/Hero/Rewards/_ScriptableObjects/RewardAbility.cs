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

            List<Ability> abilitiesAlreadyInRewardPool = new();
            foreach (RewardElement rc in otherRewardCards)
            {
                if (rc is not RewardElementAbility) continue;
                RewardAbility ra = (RewardAbility)rc.Reward;
                abilitiesAlreadyInRewardPool.Add(ra.Ability);
            }

            Ability = GetValidAbility(abilitiesAlreadyInRewardPool);
            if (Ability == null) return false;

            Ability.InitializeBattle(hero);

            foreach (Ability heroAbility in Hero.Abilities)
                if (heroAbility.Id == Ability.Id)
                {
                    IsUpgrade = true;
                    Level = heroAbility.Level + 2;
                }

            return true;
        }

        Ability GetValidAbility(List<Ability> forbiddenAbilities)
        {
            List<Ability> abilities = new(GameManager.UnitDatabase.GetAllBasicAbilities());
            if (Hero.Abilities.Count >= 4) // only 4 ability buttons // HERE: ability limit
                abilities = new(Hero.Abilities);

            abilities.AddRange(Hero.AdvancedAbilities);

            for (int i = abilities.Count - 1; i >= 0; i--)
                if (!IsAbilityValid(abilities[i], forbiddenAbilities))
                    abilities.Remove(abilities[i]);

            // TODO: possibly error if no abilities to choose from
            if (abilities.Count == 0)
            {
                Debug.LogError("Reward - no abilities to choose from");
                return null;
            }

            return abilities[Random.Range(0, abilities.Count)];
        }

        bool IsAbilityValid(Ability ability, List<Ability> forbiddenAbilities)
        {
            if (forbiddenAbilities.Contains(ability)) return false;

            Ability heroAb = Hero.GetAbilityById(ability.Id);
            if (heroAb == null) return true;
            if (heroAb.IsMaxLevel()) return false;

            return true;
        }

        public override void GetReward()
        {
            base.GetReward();
            if (IsUpgrade)
            {
                foreach (Ability heroAbility in Hero.Abilities)
                    if (heroAbility.Id == Ability.Id)
                        heroAbility.LevelUp();
                return;
            }

            Hero.AddAbility(Ability);
        }
    }
}