using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using MoreMountains.Feedbacks;

public class BattleCoin : BattlePickup
{

    Coin _coin;

    public override void Initialize(Pickup pickUp)
    {
        base.Initialize(pickUp);
        _coin = pickUp as Coin;

    }

    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);
        DisplayText($"+{_coin.Amount} gold", Color.yellow);
        GetComponentInChildren<Light>().enabled = false;
    }
}

