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

        _battleManager.OnBagCollected += OnBagCollected;
    }

    void OnBagCollected()
    {
        transform.DOMove(_battleManager.BattleHero.transform.position + Vector3.up, Random.Range(0.5f, 2f))
            .OnComplete(() =>
            {
                PickUp(_battleManager.BattleHero);
            });
    }


    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);
        _battleManager.OnBagCollected -= OnBagCollected;

        DisplayText($"+{_coin.Amount} gold", Color.yellow);
        GetComponentInChildren<Light>().enabled = false;
    }
}

