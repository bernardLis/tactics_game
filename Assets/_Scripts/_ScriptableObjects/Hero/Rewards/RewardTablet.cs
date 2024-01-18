using System.Collections.Generic;



using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
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
                availableTablets.Add(_hero.AdvancedTablet);

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
            if (_hero.AdvancedTablet == null) return false;
            if (_hero.AdvancedTablet.IsMaxLevel()) return false;
            foreach (RewardElement el in otherRewardElements)
                if (el.Reward is RewardTablet rt)
                    if (rt.Tablet.Id == _hero.AdvancedTablet.Id)
                        return false;

            return true;
        }

        public override void GetReward()
        {
            base.GetReward();

            if (Tablet is TabletAdvanced)
            {
                _hero.AdvancedTablet.LevelUp();
                return;
            }

            foreach (Tablet t in _hero.Tablets)
            {
                if (t.Id != Tablet.Id) continue;
                t.LevelUp();
                break;
            }
        }
    }
}
