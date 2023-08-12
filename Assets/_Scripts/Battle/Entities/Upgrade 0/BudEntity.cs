using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BudEntity : BattleCreatureRanged
{
    [SerializeField] GameObject _effect;
    GameObject _effectInstance;
    protected override void Start()
    {
        _hasSpecialMove = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
    {
        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);
        _effectInstance.transform.parent = transform;

        Vector3 point = ClosestPositionWithClearLOS();
        transform.position = point;

        Invoke(nameof(CleanUp), 2f);

        yield return base.SpecialAbility();
    }

    void CleanUp()
    {
        if (_effectInstance != null)
            Destroy(_effectInstance);
    }

}
