using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattlePickUp : MonoBehaviour
{
    protected AudioManager _audioManager;

    protected BattleManager _battleManager;

    SphereCollider _sphereCollider;
    Hero _hero;

    public event Action OnPickedUp;
    public virtual void Initialize(PickUp pickUp)
    {
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;

        _hero = _battleManager.GetComponent<BattleHeroManager>().BattleHero.Hero;
        _sphereCollider = GetComponent<SphereCollider>();

        SetPickUpRadius(_hero.Pull.GetValue());
        _hero.Pull.OnValueChanged += SetPickUpRadius;

        _battleManager.AddPickup(this);
    }

    protected virtual void PickUp(BattleHero hero)
    {
        GetComponent<SphereCollider>().enabled = false;
        transform.DOKill();

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
}