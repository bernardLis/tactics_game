using System.Collections.Generic;
using Lis.Battle.Fight;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    using Pawn;

    public class RewardPawn : Reward
    {
        Pawn _original;
        public Pawn Pawn;
        public int Count;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);
            _original = GameManager.UnitDatabase.GetRandomPawn();
            Pawn = Instantiate(_original);
            Pawn.InitializeBattle(0);

            Count = Random.Range(1, 4); // TODO: balance
            SetPrice();
            return true;
        }

        protected override void SetPrice()
        {
            Price = Pawn.GetCurrentUpgrade().Price * Count;
        }

        public override void GetReward()
        {
            base.GetReward();
            for (int i = 0; i < Count; i++)
            {
                Pawn instance = Instantiate(_original);
                instance.InitializeBattle(0);
                Hero.AddArmy(Instantiate(instance));
                FightManager.Instance.SpawnPlayerUnit(instance);
            }
        }
    }
}