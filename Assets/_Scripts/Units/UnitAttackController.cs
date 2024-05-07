using System;
using System.Collections;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

namespace Lis.Units
{
    public class UnitAttackController : MonoBehaviour
    {
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        AudioManager _audioManager;
        Animator _animator;

        protected UnitController UnitController;
        float _currentAttackCooldown;

        public event Action OnAttackReady;

        public virtual void Initialize(UnitController unitController)
        {
            _audioManager = AudioManager.Instance;
            _animator = GetComponentInChildren<Animator>();

            UnitController = unitController;
            _currentAttackCooldown = unitController.Unit.CurrentAttack.Cooldown;
        }

        void Update()
        {
            if (_currentAttackCooldown >= 0)
                _currentAttackCooldown -= Time.deltaTime;
        }

        bool CanAttack()
        {
            return _currentAttackCooldown <= 0;
        }

        protected IEnumerator BaseAttackCoroutine()
        {
            while (!CanAttack()) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;
            OnAttackReady?.Invoke();

            UnitController.AddToLog($"Unit attacks {UnitController.Opponent.name}");
            _currentAttackCooldown = UnitController.Unit.CurrentAttack.Cooldown;

            if (UnitController.Unit.AttackSound != null)
                _audioManager.PlaySfx(UnitController.Unit.AttackSound, transform.position);
            yield return transform.DODynamicLookAt(UnitController.Opponent.transform.position, 0.2f, AxisConstraint.Y);
            _animator.SetTrigger(AnimAttack);

            bool isAttack = false;
            while (true)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack"))
                    isAttack = true;
                bool isAttackFinished = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f;

                if (isAttack && isAttackFinished) break;

                yield return new WaitForSeconds(0.1f);
            }
        }

        public virtual IEnumerator AttackCoroutine()
        {
            yield return BaseAttackCoroutine();
            yield return UnitController.Opponent.GetHit(UnitController.Unit.CurrentAttack);
        }

        public bool IsOpponentInRange()
        {
            if (UnitController.Opponent == null) return false;
            if (UnitController.Opponent.IsDead) return false;

            // +0.5 wiggle room
            Vector3 delta = UnitController.Opponent.transform.position - transform.position;
            float distanceSqrt = delta.sqrMagnitude;
            float attackRangeSqrt = (UnitController.Unit.CurrentAttack.Range + 0.5f) *
                                    (UnitController.Unit.CurrentAttack.Range + 0.5f);
            return distanceSqrt <= attackRangeSqrt;
        }
    }
}