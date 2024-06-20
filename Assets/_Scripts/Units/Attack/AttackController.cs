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
        IEnumerator _attackCooldownCoroutine;
        protected Animator Animator;

        protected Attack Attack;

        protected AudioManager AudioManager;

        protected UnitController UnitController;
        public float CurrentCooldown { get; private set; }

        public virtual void Initialize(UnitController unitController, Attack attack)
        {
            AudioManager = AudioManager.Instance;
            Animator = unitController.GetComponentInChildren<Animator>();

            Attack = attack;

            UnitController = unitController;
        }

        public void ResetCooldown()
        {
            CurrentCooldown = 0;
        }

        public void SetAnimator(Animator animator)
        {
            Animator = animator;
        }

        protected void BaseAttack()
        {
            UnitController.AddToLog($"Unit attacks {UnitController.Opponent.name} with {Attack.name}");
            StartCoroutine(UnitController.GlobalAttackCooldownCoroutine(Attack.GlobalCooldown));
            if (_attackCooldownCoroutine != null) StopCoroutine(_attackCooldownCoroutine);
            _attackCooldownCoroutine = AttackCooldownCoroutine(Attack.Cooldown);
            StartCoroutine(_attackCooldownCoroutine);
        }

        IEnumerator AttackCooldownCoroutine(float cooldown)
        {
            CurrentCooldown = cooldown;
            while (CurrentCooldown > 0)
            {
                CurrentCooldown -= 1;
                yield return new WaitForSeconds(1f);
            }
        }

        protected IEnumerator BasicAttackCoroutine()
        {
            if (UnitController.Unit.AttackSound != null)
                AudioManager.PlaySound(UnitController.Unit.AttackSound, transform.position);
            yield return UnitController.transform.DODynamicLookAt(UnitController.Opponent.transform.position,
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
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;

            BaseAttack();
            yield return BasicAttackCoroutine();

            if (UnitController.Opponent == null) yield break;
            yield return UnitController.Opponent.GetHit(Attack);
        }

        protected bool IsOpponentInRange()
        {
            if (UnitController.Opponent == null) return false;
            if (UnitController.Opponent.IsDead) return false;

            // +0.5 wiggle room
            Vector3 delta = UnitController.Opponent.transform.position - transform.position;
            float distanceSqrt = delta.sqrMagnitude;
            float attackRangeSqrt = (Attack.Range + 1.5f) *
                                    (Attack.Range + 1.5f);
            return distanceSqrt <= attackRangeSqrt;
        }

        protected List<UnitController> GetOpponentsInRadius(float radius)
        {
            var opponents = new List<UnitController>();
            var colliders = new Collider[25];
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