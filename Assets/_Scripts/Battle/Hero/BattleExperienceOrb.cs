using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using MoreMountains.Feedbacks;

public class BattleExperienceOrb : BattlePickup
{
    ExperienceOrb _expOrb;

    public override void Initialize(Pickup pickUp)
    {
        base.Initialize(pickUp);

        _expOrb = pickUp as ExperienceOrb;

        transform.position = new Vector3(
            transform.position.x,
            0.5f,
            transform.position.z
        );

        _battleManager.OnHorseshoeCollected += OnMagnetCollected;

        // GetComponentInChildren<Light>().color = _expOrb.Color.Color;
    }

    void OnMagnetCollected()
    {
        transform.DOMove(_battleManager.BattleHero.transform.position, Random.Range(0.5f, 2f)).OnComplete(
            () => { PickUp(_battleManager.BattleHero); });

    }

    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);
        _battleManager.OnHorseshoeCollected -= OnMagnetCollected;

        DisplayText($"+{_expOrb.Amount} EXP", _expOrb.Color.Color);
    }
}

