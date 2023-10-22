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
        if (IsShielded) yield break;

        yield return base.CreatureAbility();

        DisplayFloatingText("Shielded", Color.blue);
        _shieldEffectInstance = Instantiate(_shieldEffect, transform.position, Quaternion.identity);
        _shieldEffectInstance.transform.parent = _GFX.transform;

        IsShielded = true;
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

    public override IEnumerator GetHit(EntityFight attacker, int specialDamage = 0)
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
