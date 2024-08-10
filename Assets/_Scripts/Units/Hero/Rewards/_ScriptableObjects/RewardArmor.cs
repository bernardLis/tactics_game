using System.Collections.Generic;
using Lis.Units.Hero.Items;

namespace Lis.Units.Hero.Rewards
{
    public class RewardArmor : Reward
    {
        public Armor Armor;

        public override bool CreateRandom(Hero hero, List<Reward> otherRewards)
        {
            base.CreateRandom(hero, otherRewards);
            if (hero.VisualHero.BodyType == 0)
                Armor = Instantiate(GameManager.UnitDatabase.GetRandomFemaleArmor());
            if (hero.VisualHero.BodyType == 1)
                Armor = Instantiate(GameManager.UnitDatabase.GetRandomMaleArmor());

            SetPrice();
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