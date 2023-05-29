using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShellEntity : BattleEntityMelee
{
    [SerializeField] GameObject _shieldEffect;
    GameObject _shieldEffectInstance;

    protected override void Start()
    {
        _hasSpecialAction = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
    {
        if (IsShielded) yield return base.SpecialAbility();

        DisplayFloatingText("Shielded", Color.blue);

        _shieldEffectInstance = Instantiate(_shieldEffect, transform.position, Quaternion.identity);
        _shieldEffectInstance.transform.parent = _GFX.transform;

        IsShielded = true;

        yield return base.SpecialAbility();
    }

    public override IEnumerator GetHit(Ability ability)
    {
        if (IsShielded)
        {
            BreakShield();
            yield break;
        }
        yield return base.GetHit(ability);
    }

    public override IEnumerator GetHit(BattleEntity attacker)
    {
        if (IsShielded)
        {
            BreakShield();
            yield break;
        }

        yield return base.GetHit(attacker);
    }

    public void BreakShield()
    {
        DisplayFloatingText("Shield Broken", Color.blue);
        IsShielded = false;
        if (_shieldEffectInstance != null)
            Destroy(_shieldEffectInstance);
    }
}
