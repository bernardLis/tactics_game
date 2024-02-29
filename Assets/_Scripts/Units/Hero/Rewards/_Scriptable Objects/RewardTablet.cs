using System.Collections.Generic;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Rewards
{
    public class RewardTablet : Reward
    {
        public Tablet Tablet;

        public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardElements)
        {
            base.CreateRandom(hero, otherRewardElements);

            List<Tablet> heroTablets = new(hero.Tablets);
            List<Tablet> availableTablets = new();
            foreach (Tablet tablet in heroTablets)
                if (!tablet.IsMaxLevel())
                    availableTablets.Add(tablet);

            foreach (RewardElement rc in otherRewardElements)
            {
                if (rc is not RewardElementTablet) continue;
                RewardTablet r = (RewardTablet)rc.Reward;
                availableTablets.Remove(r.Tablet);
            }

            if (CanUpgradeAdvancedTablet(otherRewardElements))
                availableTablets.Add(Hero.AdvancedTablet);

            if (availableTablets.Count == 0)
            {
                Debug.LogError("Reward - no tablet to upgrade");
                return false;
            }

            Tablet = availableTablets[Random.Range(0, availableTablets.Count)];
            return true;
        }

        bool CanUpgradeAdvancedTablet(List<RewardElement> otherRewardElements)
        {
            if (Hero.AdvancedTablet == null) return false;
            if (Hero.AdvancedTablet.IsMaxLevel()) return false;
            foreach (RewardElement el in otherRewardElements)
                if (el.Reward is RewardTablet rt)
                    if (rt.Tablet.Id == Hero.AdvancedTablet.Id)
                        return false;

            return true;
        }

        public override void GetReward()
        {
            base.GetReward();

            if (Tablet is TabletAdvanced)
            {
                Hero.AdvancedTablet.LevelUp();
                return;
            }

            foreach (Tablet t in Hero.Tablets)
            {
                if (t.Id != Tablet.Id) continue;
                t.LevelUp();
                break;
            }
        }
    }
}
