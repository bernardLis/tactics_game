using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PracticeDummyEntity : BattleCreatureMelee
{
    [SerializeField] float _specialEffectRadius = 3f;
    [SerializeField] GameObject _specialEffect;

    GameObject _specialEffectInstance;

    List<GameObject> _hitInstances = new();

    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        if (!IsOpponentInRange()) yield break;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown;

        _specialEffectInstance = Instantiate(_specialEffect, transform.position, Quaternion.identity);
        _specialEffectInstance.transform.parent = _GFX.transform;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _specialEffectRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(this, Creature.GetPower() * 2));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(Creature.HitPrefab, Opponent.Collider.bounds.center, q);
                hitInstance.transform.parent = Opponent.transform;
                _hitInstances.Add(hitInstance);
            }
        }

        Invoke(nameof(CleanUp), 2f);
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
