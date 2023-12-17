using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RewardTablet : Reward
{
    public Tablet Tablet;

    public override bool CreateRandom(Hero hero, List<RewardElement> otherRewardCards)
    {
        base.CreateRandom(hero, otherRewardCards);

        List<Tablet> heroTablets = new(hero.Tablets);
        List<Tablet> availableTablets = new();
        foreach (Tablet tablet in heroTablets)
            if (!tablet.IsMaxLevel())
                availableTablets.Add(tablet);

        foreach (RewardElement rc in otherRewardCards)
        {
            if (rc is not RewardElementTablet) continue;
            RewardTablet r = (RewardTablet)rc.Reward;
            availableTablets.Remove(r.Tablet);
        }

        if(_hero.AdvancedTablet != null && !_hero.AdvancedTablet.IsMaxLevel())
            availableTablets.Add(_hero.AdvancedTablet);

        if (availableTablets.Count == 0)
        {
            Debug.LogError("Reward - no tablet to upgrade");
            return false;
        }

        Tablet = availableTablets[Random.Range(0, availableTablets.Count)];
        return true;
    }


    public override void GetReward()
    {
        base.GetReward();
        foreach (Tablet t in _hero.Tablets)
        {
            if (t.Id != Tablet.Id) continue;
            t.LevelUp();
            break;
        }
    }
}
