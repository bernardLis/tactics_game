using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattlePracticeDummy : BattleCreatureMelee
    {
        [SerializeField] float _abilityEffectRadius = 3f;
        [SerializeField] GameObject _abilityEffect;

        GameObject _abilityEffectInstance;

        readonly List<GameObject> _hitInstances = new();

        protected override IEnumerator Attack()
        {
            yield return ManageCreatureAbility();
            yield return base.Attack();
        }

        protected override IEnumerator CreatureAbility()
        {
            if (!IsOpponentInRange()) yield break;
            Vector3 pos = transform.position;
            pos.y = 0;
            _abilityEffectInstance = Instantiate(_abilityEffect, pos, Quaternion.identity);
            _abilityEffectInstance.transform.parent = Gfx.transform;

            transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
            StartCoroutine(base.CreatureAbility());
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

            Collider[] results = new Collider[10];
            Physics.OverlapSphereNonAlloc(pos, _abilityEffectRadius, results);
            foreach (Collider r in results)
            {
                if (r == null) continue;
                if (!r.TryGetComponent(out BattleEntity entity)) continue;
                if (entity.Team == Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(this, Mathf.FloorToInt(Creature.Power.GetValue() * 2)));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(Creature.HitPrefab, entity.Collider.bounds.center, q);
                hitInstance.transform.parent = Opponent.transform;
                _hitInstances.Add(hitInstance);
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
}