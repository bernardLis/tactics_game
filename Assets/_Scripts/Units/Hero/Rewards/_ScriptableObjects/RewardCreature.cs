using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    using Creature;

    public class RewardCreature : Reward
    {
        Creature _original;
        public Creature Creature;
        public int Count;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);
            _original = GameManager.UnitDatabase.GetRandomCreature();
            Creature = Instantiate(_original);
            Creature.InitializeBattle(0);

            Count = Random.Range(1, 4); // TODO: for now
            SetPrice();
            return true;
        }

        protected override void SetPrice()
        {
            Price = Creature.Price * Count;
        }

        public override void GetReward()
        {
            base.GetReward();
            for (int i = 0; i < Count; i++)
            {
                Creature instance = Instantiate(_original);
                instance.InitializeBattle(0);
                Hero.AddArmy(Instantiate(instance));
            }
        }
    }
}