using System.Collections.Generic;
using Lis.Battle.Fight;
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
            _original = ChooseCreature(otherRewardCards);
            Creature = Instantiate(_original);
            Creature.InitializeBattle(0);

            Count = 1; // TODO: balance
            SetPrice();
            return true;
        }

        Creature ChooseCreature(List<RewardElement> otherRewardCards)
        {
            // make sure no duplicate creatures in the rewards
            List<Creature> creatures = new(GameManager.UnitDatabase.AllCreatures);
            List<Creature> creaturesToRemove = new();
            foreach (RewardElement el in otherRewardCards)
                if (el.Reward is RewardCreature rewardCreature)
                    foreach (Creature c in creatures)
                        if (c.Id == rewardCreature.Creature.Id)
                            creaturesToRemove.Add(c);

            foreach (Creature c in creaturesToRemove)
                creatures.Remove(c);

            return creatures[Random.Range(0, creatures.Count)];
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
                FightManager.Instance.SpawnPlayerUnit(instance);
            }
        }
    }
}