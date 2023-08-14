using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SunBlossomEntity : BattleCreatureMelee
{
    [SerializeField] GameObject _healEffect;
    GameObject _healEffectInstance;

    //TODO: I'd prefer if it used its ability whenever it is off cooldown, it is not shielded and ability is available
    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator PathToOpponent()
    {
        yield return ManageCreatureAbility();
        yield return base.PathToOpponent();
    }

    protected override IEnumerator CreatureAbility()
    {
        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        List<BattleEntity> copyOfAllies = new(BattleManager.Instance.GetAllies(this));
        bool hasHealed = false;
        foreach (BattleEntity b in copyOfAllies)
        {
            if (b.HasFullHealth()) continue;
            if (b.IsDead) continue;
            hasHealed = true;
            b.GetHealed(20); // TODO: hardcoded value
        }

        if (!hasHealed)
            GetHealed(20); // TODO: hardcoded value

        if (_creatureAbilitySound != null) _audioManager.PlaySFX(_creatureAbilitySound, transform.position);

        _healEffectInstance = Instantiate(_healEffect, transform.position, Quaternion.identity);
        _healEffectInstance.transform.parent = _GFX.transform;

        yield return base.CreatureAbility();
    }
}
