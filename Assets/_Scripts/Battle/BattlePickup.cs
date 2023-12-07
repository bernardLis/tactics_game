using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class BattlePickup : MonoBehaviour
{
    protected AudioManager _audioManager;
    protected BattleManager _battleManager;
    MMF_Player _feelPlayer;


    SphereCollider _sphereCollider;
    Hero _hero;

    Pickup _pickUp;

    public event Action OnPickedUp;
    public virtual void Initialize(Pickup pickUp)
    {
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;

        _feelPlayer = GetComponent<MMF_Player>();

        _pickUp = pickUp;
        _pickUp.Initialize();

        _hero = _battleManager.GetComponent<BattleHeroManager>().BattleHero.Hero;
        _sphereCollider = GetComponent<SphereCollider>();

        SetPickUpRadius(_hero.Pull.GetValue());
        _hero.Pull.OnValueChanged += SetPickUpRadius;

        _battleManager.AddPickup(this);
        transform.parent = _battleManager.EntityHolder;

        _audioManager.PlaySFX(pickUp.DropSound, transform.position);

        transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.OutBack);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, 1f).SetEase(Ease.OutBack);

        transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
                 .SetLoops(-1).SetEase(Ease.InOutSine);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;

        PickUp(hero);
    }

    protected virtual void PickUp(BattleHero hero)
    {
        GetComponent<SphereCollider>().enabled = false;
        transform.DOKill();

        _audioManager.PlaySFX(_pickUp.CollectSound, transform.position);
        Destroy(Instantiate(_pickUp.CollectEffect, transform.position, Quaternion.identity), 1f);

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
                    _pickUp.Collected(hero.Hero);
                    Destroy(gameObject, 1f);
                });

        OnPickedUp?.Invoke();
    }

    void OnDestroy()
    {
        _hero.Pull.OnValueChanged -= SetPickUpRadius;
    }

    void SetPickUpRadius(int i)
    {
        _sphereCollider.radius = i;
    }

    protected void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}