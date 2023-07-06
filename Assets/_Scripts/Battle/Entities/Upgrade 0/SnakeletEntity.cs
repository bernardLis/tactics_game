using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Snakelet : BattleEntityMelee
{

    [SerializeField] GameObject _specialHit;
    GameObject _specialHitInstance;

    protected override void Start()
    {
        _hasSpecialAttack = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
    {
        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

        _specialHitInstance = Instantiate(_specialHit, Opponent.transform.position, Quaternion.identity);
        _specialHitInstance.transform.parent = Opponent.transform;
        StartCoroutine(Opponent.GetPoisoned(this));

        Invoke("CleanUp", 2f);

        yield return base.SpecialAbility();

    }

    void CleanUp()
    {
        if (_specialHitInstance != null)
            Destroy(_specialHitInstance);

    }
}
