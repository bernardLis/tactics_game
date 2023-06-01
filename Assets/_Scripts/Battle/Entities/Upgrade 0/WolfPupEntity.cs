using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WolfPupEntity : BattleEntityMelee
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
        transform.DODynamicLookAt(_opponent.transform.position, 0.2f, AxisConstraint.Y);

        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);

        Vector3 normal = (_opponent.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + normal * 10f;

        // if opp is in range, jump behind him not *10f
        if (IsOpponentInRange())
        {
            targetPosition = transform.position + normal * (_agent.stoppingDistance * 2);
            StartCoroutine(_opponent.GetHit(this, (int)this.ArmyEntity.Power * 3));
        }
        transform.DOJump(targetPosition, 2f, 1, 0.3f, false);

        Invoke("CleanUp", 2f);

        yield return base.SpecialAbility();
    }

    void CleanUp()
    {
        if (_effectInstance != null)
            Destroy(_effectInstance);
    }

}
