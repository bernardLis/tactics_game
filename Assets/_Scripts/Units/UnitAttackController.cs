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

        [SerializeField] Sound _attackSound;

        UnitController _unitController;
        float _currentAttackCooldown;

        public event Action OnAttackReady;

        public void Initialize(UnitController unitController)
        {
            _audioManager = AudioManager.Instance;
            _animator = GetComponentInChildren<Animator>();

            _unitController = unitController;
            _currentAttackCooldown = unitController.Unit.AttackCooldown.GetValue();
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

        public IEnumerator AttackCoroutine()
        {
            while (!CanAttack()) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;
            OnAttackReady?.Invoke();

            _unitController.AddToLog($"Unit attacks {_unitController.Opponent.name}");
            _currentAttackCooldown = _unitController.Unit.AttackCooldown.GetValue();

            if (_attackSound != null) _audioManager.PlaySfx(_attackSound, transform.position);
            yield return transform.DODynamicLookAt(_unitController.Opponent.transform.position, 0.2f, AxisConstraint.Y);
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

            yield return _unitController.Opponent.GetHit(_unitController);
        }

        public bool IsOpponentInRange()
        {
            if (_unitController.Opponent == null) return false;
            if (_unitController.Opponent.IsDead) return false;

            // +0.5 wiggle room
            Vector3 delta = _unitController.Opponent.transform.position - transform.position;
            float distanceSqrt = delta.sqrMagnitude;
            float attackRangeSqrt = (_unitController.Unit.AttackRange.GetValue() + 0.5f) *
                                    (_unitController.Unit.AttackRange.GetValue() + 0.5f);
            return distanceSqrt <= attackRangeSqrt;
        }
    }
}