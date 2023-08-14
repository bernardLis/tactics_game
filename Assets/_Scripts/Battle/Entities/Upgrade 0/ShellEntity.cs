using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShellEntity : BattleCreatureMelee
{
    [SerializeField] GameObject _shieldEffect;
    GameObject _shieldEffectInstance;

    //TODO: it is not an ideal approach
    // I'd prefer if shell used its ability whenever it is off cooldown, it is not shielded and ability is available
    protected override IEnumerator Attack()
    {
        if (!IsShielded)
            yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator PathToOpponent()
    {
        if (!IsShielded)
            yield return ManageCreatureAbility();
        yield return base.PathToOpponent();
    }

    void OnDisabled()
    {
        if (_shieldEffectInstance != null)
            Destroy(_shieldEffectInstance);
    }

    protected override IEnumerator CreatureAbility()
    {
        if (IsShielded)
        {
            yield return base.CreatureAbility();
            yield break;
        }

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (_creatureAbilitySound != null) _audioManager.PlaySFX(_creatureAbilitySound, transform.position);

        DisplayFloatingText("Shielded", Color.blue);
        _shieldEffectInstance = Instantiate(_shieldEffect, transform.position, Quaternion.identity);
        _shieldEffectInstance.transform.parent = _GFX.transform;

        IsShielded = true;

        yield return base.CreatureAbility();
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

    public override IEnumerator GetHit(BattleCreature attacker, int specialDamage = 0)
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

    protected override void Evolve()
    {
        if (_shieldEffectInstance != null)
            Destroy(_shieldEffectInstance);
        base.Evolve();
    }
}
