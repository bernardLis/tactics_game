using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PracticeDummyEntity : BattleEntityMelee
{
    [SerializeField] float _specialEffectRadius = 3f;
    [SerializeField] GameObject _specialEffect;

    GameObject _specialEffectInstance;

    List<GameObject> _hitInstances = new();

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

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Special Attack");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        if (_specialAbilitySound != null) _audioManager.PlaySFX(_specialAbilitySound, transform.position);

        _specialEffectInstance = Instantiate(_specialEffect, transform.position, Quaternion.identity);
        _specialEffectInstance.transform.parent = _GFX.transform;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _specialEffectRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<BattleEntity>(out BattleEntity entity))
            {
                if (entity.Team == Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(this, (int)this.Creature.GetPower() * 2));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(Creature.HitPrefab, _opponent.Collider.bounds.center, q);
                hitInstance.transform.parent = _opponent.transform;
                _hitInstances.Add(hitInstance);
            }
        }

        Invoke("CleanUp", 2f);

        yield return base.SpecialAbility();

    }

    void CleanUp()
    {
        if (_specialEffectInstance != null)
            Destroy(_specialEffectInstance);

        for (int i = _hitInstances.Count - 1; i >= 0; i--)
        {
            Destroy(_hitInstances[i]);
            _hitInstances.RemoveAt(i);
        }
    }
}
