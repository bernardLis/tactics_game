using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PracticeDummyEntity : BattleCreatureMelee
{
    [SerializeField] float _abilityEffectRadius = 3f;
    [SerializeField] GameObject _abilityEffect;

    GameObject _abilityEffectInstance;

    List<GameObject> _hitInstances = new();

    protected override IEnumerator Attack()
    {
        yield return ManageCreatureAbility();
        yield return base.Attack();
    }

    protected override IEnumerator CreatureAbility()
    {
        Debug.Log($"ability");
        if (!IsOpponentInRange()) yield break;

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        yield return base.CreatureAbility();
        _currentAttackCooldown = Creature.AttackCooldown.GetValue();

        _abilityEffectInstance = Instantiate(_abilityEffect, transform.position, Quaternion.identity);
        _abilityEffectInstance.transform.parent = _GFX.transform;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _abilityEffectRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(Creature, Creature.Power.GetValue() * 2));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(Creature.HitPrefab, entity.Collider.bounds.center, q);
                hitInstance.transform.parent = Opponent.transform;
                _hitInstances.Add(hitInstance);
            }
        }

        Invoke(nameof(CleanUp), 2f);
    }

    void CleanUp()
    {
        if (_abilityEffectInstance != null)
            Destroy(_abilityEffectInstance);

        for (int i = _hitInstances.Count - 1; i >= 0; i--)
        {
            Destroy(_hitInstances[i]);
            _hitInstances.RemoveAt(i);
        }
    }
}
