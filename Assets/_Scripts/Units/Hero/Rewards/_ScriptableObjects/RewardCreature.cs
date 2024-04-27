using System.Collections.Generic;
using Lis.Battle.Fight;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    using Creature;

    public class RewardCreature : Reward
    {
        public Creature Creature;
        public int Count;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);

            Creature = Instantiate(GameManager.UnitDatabase.GetRandomCreature());
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
                Hero.AddArmy(Creature);
        }
    }
}