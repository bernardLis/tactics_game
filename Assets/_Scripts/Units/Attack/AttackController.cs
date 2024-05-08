using System;
using System.Collections;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackController : MonoBehaviour
    {
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        AudioManager _audioManager;
        Animator _animator;

        protected UnitController UnitController;

        protected Attack Attack;

        public event Action OnAttackReady;

        public virtual void Initialize(UnitController unitController, Attack attack)
        {
            _audioManager = AudioManager.Instance;
            _animator = unitController.GetComponentInChildren<Animator>();

            Attack = attack;

            UnitController = unitController;
        }

        protected IEnumerator BaseAttackCoroutine()
        {
            if (!IsOpponentInRange()) yield break;
            OnAttackReady?.Invoke();

            UnitController.AddToLog($"Unit attacks {UnitController.Opponent.name}");
            StartCoroutine(UnitController.StartAttackCooldown(Attack.Cooldown));

            if (UnitController.Unit.AttackSound != null)
                _audioManager.PlaySfx(UnitController.Unit.AttackSound, transform.position);
            yield return transform.DODynamicLookAt(UnitController.Opponent.transform.position,
                0.2f, AxisConstraint.Y);
            _animator.SetTrigger(AnimAttack);

            bool isAttack = false;
            while (true)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack"))
                    isAttack = true;
                bool isAttackFinished =
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f;

                if (isAttack && isAttackFinished) break;

                yield return new WaitForSeconds(0.1f);
            }
        }

        public virtual IEnumerator AttackCoroutine()
        {
            yield return BaseAttackCoroutine();
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
    }
}