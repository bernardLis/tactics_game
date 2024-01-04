using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using MoreMountains.Feedbacks;

public class BattleCoin : BattlePickup
{
    Coin _coin;

    // public override void Initialize(Pickup pickUp)
    // {
    //     Pickup instance = Instantiate(pickUp); // coz of pooling

    //     base.Initialize(instance);
    //     _coin = instance as Coin;

    //     _battleManager.OnBagCollected += OnBagCollected;
    // }

    public void Spawn(Vector3 pos)
    {
        transform.position = pos;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    void OnBagCollected()
    {
        if (!gameObject.activeSelf) return;

        transform.DOMove(_battleManager.BattleHero.transform.position + Vector3.up, Random.Range(0.5f, 2f))
            .OnComplete(() =>
            {
                PickUp(_battleManager.BattleHero);
            });
    }


    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);
        // _battleManager.OnBagCollected -= OnBagCollected;

        DisplayText($"+{_coin.Amount} gold", Color.yellow);
    }
}

