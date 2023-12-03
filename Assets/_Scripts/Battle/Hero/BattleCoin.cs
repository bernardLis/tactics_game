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

        transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.OutBack);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, 1f).SetEase(Ease.OutBack);

        transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
                 .SetLoops(-1).SetEase(Ease.InOutSine);
    }

    protected override void PickUp(BattleHero hero)
    {
        Debug.Log($"pickup");
        base.PickUp(hero);
        DisplayText($"+{_coin.Amount}", Color.yellow);
        GetComponentInChildren<Light>().enabled = false;
    }
}

