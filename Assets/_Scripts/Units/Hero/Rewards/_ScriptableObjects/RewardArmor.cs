using System.Collections.Generic;
using Lis.Units.Hero.Items;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    public class RewardArmor : Reward
    {
        public Armor Armor;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardElements)
        {
            base.CreateRandom(hero, otherRewardElements);
            if (hero.VisualHero.BodyType == 0)
                Armor = Instantiate(GameManager.UnitDatabase.GetRandomFemaleArmor());
            if (hero.VisualHero.BodyType == 1)
                Armor = Instantiate(GameManager.UnitDatabase.GetRandomMaleArmor());

            return true;
        }

        protected override void SetPrice()
        {
            Price = Armor.Price;
        }

        public override void GetReward()
        {
            base.GetReward();
            Hero.AddArmor(Armor);
        }
    }
}