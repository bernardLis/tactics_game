using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SunBlossomEntity : BattleCreatureMelee
{
    [SerializeField] GameObject _healEffect;
    GameObject _healEffectInstance;

    protected override void Start()
    {
        _hasSpecialAction = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
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

        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

        _healEffectInstance = Instantiate(_healEffect, transform.position, Quaternion.identity);
        _healEffectInstance.transform.parent = _GFX.transform;

        yield return base.SpecialAbility();
    }
}
