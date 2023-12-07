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

        // GetComponentInChildren<Light>().color = _expOrb.Color.Color;
    }

    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);

        DisplayText($"+{_expOrb.Amount} EXP", _expOrb.Color.Color);
    }
}

