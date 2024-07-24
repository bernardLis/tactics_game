using System.Collections.Generic;
using Lis.Arena.Fight;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    public class RewardPawn : Reward
    {
        public Pawn.Pawn Pawn;
        public int Count;
        Pawn.Pawn _original;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
        {
            base.CreateRandom(hero, otherRewardCards);
            _original = ChoosePawn(otherRewardCards);
            Pawn = Instantiate(_original);
            Pawn.InitializeFight(0);

            Count = Random.Range(1, 4); // TODO: balance
            SetPrice();
            return true;
        }

        Pawn.Pawn ChoosePawn(List<RewardElement> otherRewardCards)
        {
            // make sure no duplicate pawns in the rewards
            List<Pawn.Pawn> pawns = new(GameManager.UnitDatabase.GetAllPawns());
            List<Pawn.Pawn> pawnsToRemove = new();
            foreach (RewardElement el in otherRewardCards)
                if (el.Reward is RewardPawn rewardPawn)
                    foreach (Pawn.Pawn p in pawns)
                        if (p.Id == rewardPawn.Pawn.Id)
                            pawnsToRemove.Add(p);

            foreach (Pawn.Pawn p in pawnsToRemove)
                pawns.Remove(p);

            return pawns[Random.Range(0, pawns.Count)];
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
                Pawn.Pawn instance = Instantiate(_original);
                instance.InitializeFight(0);
                Hero.AddArmy(Instantiate(instance));
                FightManager.Instance.SpawnPlayerUnit(instance);
            }
        }
    }
}