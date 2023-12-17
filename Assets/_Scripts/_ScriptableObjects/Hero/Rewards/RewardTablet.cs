using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using JetBrains.Annotations;

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

        if (CanUpgradeAdvancedTablet() && !availableTablets.Contains(_hero.AdvancedTablet))
            availableTablets.Add(_hero.AdvancedTablet);

        if (availableTablets.Count == 0)
        {
            Debug.LogError("Reward - no tablet to upgrade");
            return false;
        }

        Tablet = availableTablets[Random.Range(0, availableTablets.Count)];
        return true;
    }

    bool CanUpgradeAdvancedTablet()
    {
        if (_hero.AdvancedTablet == null) return false;
        if (_hero.AdvancedTablet.IsMaxLevel()) return false;
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
