using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;
using MoreMountains.Feedbacks;

public class BattleExperienceOrb : BattlePickup
{
    MMF_Player _feelPlayer;

    ExperienceOrb _expOrb;

    public override void Initialize(Pickup pickUp)
    {
        base.Initialize(pickUp);

        transform.parent = _battleManager.EntityHolder;

        _expOrb = pickUp as ExperienceOrb;

        _audioManager.PlaySFX(_expOrb.DropSound, transform.position);

        _feelPlayer = GetComponent<MMF_Player>();

        transform.position = new Vector3(
            transform.position.x,
            0.5f,
            transform.position.z
        );

        GetComponentInChildren<Light>().color = _expOrb.Color.Color;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;

        PickUp(hero);
    }

    protected override void PickUp(BattleHero hero)
    {
        base.PickUp(hero);

        _audioManager.PlaySFX(_expOrb.CollectSound, transform.position);
        Destroy(Instantiate(_expOrb.CollectEffect, transform.position, Quaternion.identity), 1f);
        DisplayText($"+{_expOrb.Amount} EXP", _expOrb.Color.Color);

        float punchDuration = 0.5f;
        transform.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1, 1f);

        Vector3 jumpPos = new Vector3(
            hero.transform.position.x,
            hero.transform.position.y + 2f,
            hero.transform.position.z
        );
        float jumpDuration = 0.6f;

        transform.DOScale(0, jumpDuration)
            .SetDelay(punchDuration);

        transform.DOJump(jumpPos, 5f, 1, jumpDuration)
                .SetDelay(punchDuration)
                .OnComplete(() =>
                {
                    _expOrb.Collected(hero.Hero);

                    Destroy(gameObject, 1f);
                });
    }

    void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}

