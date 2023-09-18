using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePickUp : MonoBehaviour
{
    SphereCollider _sphereCollider;
    Hero _hero;
    public virtual void Initialize(PickUp pickUp)
    {
        _hero = BattleManager.Instance.GetComponent<BattleHeroManager>().BattleHero.Hero;
        _sphereCollider = GetComponent<SphereCollider>();

        SetPickUpRadius(_hero.GatherStrength.GetValue());
        _hero.GatherStrength.OnValueChanged += SetPickUpRadius;
    }

    void OnDestroy()
    {
        _hero.GatherStrength.OnValueChanged -= SetPickUpRadius;
    }

    void SetPickUpRadius(int i)
    {
        _sphereCollider.radius = i;
    }
}