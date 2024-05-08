using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Battle.Pickup;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackController : MonoBehaviour
    {
        static readonly int AnimAttack = Animator.StringToHash("Attack");
        protected static readonly int AnimSpecialAttack = Animator.StringToHash("Special Attack");

        protected AudioManager AudioManager;
        protected Animator Animator;

        protected UnitController UnitController;

        protected Attack Attack;

        public event Action OnAttackReady;

        public virtual void Initialize(UnitController unitController, Attack attack)
        {
            AudioManager = AudioManager.Instance;
            Animator = unitController.GetComponentInChildren<Animator>();

            Attack = attack;

            UnitController = unitController;
        }

        protected void BaseAttack()
        {
            OnAttackReady?.Invoke();

            UnitController.AddToLog($"Unit attacks {UnitController.Opponent.name} with {Attack.name}");
            StartCoroutine(UnitController.StartAttackCooldown(Attack.Cooldown));
        }

        protected IEnumerator BasicAttackCoroutine()
        {
            if (UnitController.Unit.AttackSound != null)
                AudioManager.PlaySfx(UnitController.Unit.AttackSound, transform.position);
            yield return transform.DODynamicLookAt(UnitController.Opponent.transform.position,
                0.2f, AxisConstraint.Y);
            Animator.SetTrigger(AnimAttack);

            bool isAttack = false;
            while (true)
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack"))
                    isAttack = true;
                bool isAttackFinished =
                    Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f;

                if (isAttack && isAttackFinished) break;

                yield return new WaitForSeconds(0.1f);
            }
        }

        public virtual IEnumerator AttackCoroutine()
        {
            if (!IsOpponentInRange()) yield break;

            BaseAttack();
            yield return BasicAttackCoroutine();

            yield return UnitController.Opponent.GetHit(Attack);
        }

        public bool IsOpponentInRange()
        {
            if (UnitController.Opponent == null) return false;
            if (UnitController.Opponent.IsDead) return false;

            // +0.5 wiggle room
            Vector3 delta = UnitController.Opponent.transform.position - transform.position;
            float distanceSqrt = delta.sqrMagnitude;
            float attackRangeSqrt = (Attack.Range + 0.5f) *
                                    (Attack.Range + 0.5f);
            return distanceSqrt <= attackRangeSqrt;
        }

        protected List<UnitController> GetOpponentsInRadius(float radius)
        {
            List<UnitController> opponents = new List<UnitController>();
            Collider[] colliders = new Collider[25];
            Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
            foreach (Collider c in colliders)
            {
                if (c == null) continue;
                if (c.TryGetComponent(out BreakableVaseController bbv))
                {
                    bbv.TriggerBreak();
                    continue;
                }

                if (!c.TryGetComponent(out UnitController entity)) continue;
                if (entity.Team == UnitController.Unit.Team) continue;
                if (entity.IsDead) continue;
                opponents.Add(entity);
            }

            return opponents;
        }
    }
}