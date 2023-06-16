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
        if (IsShielded)
        {
            yield return base.SpecialAbility();
            yield break;
        }

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

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

    public override IEnumerator GetHit(BattleEntity attacker, int specialDamage = 0)
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
