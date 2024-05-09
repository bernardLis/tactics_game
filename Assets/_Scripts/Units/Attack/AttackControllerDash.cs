using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerDash : AttackController
    {
        [SerializeField] GameObject _effect;

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;
            BaseAttack();

            Vector3 transformPosition = transform.position;
            Vector3 oppPosition = UnitController.Opponent.transform.position;
            yield return UnitController.transform.DODynamicLookAt(oppPosition, 0.2f, AxisConstraint.Y)
                .WaitForCompletion();

            _effect.SetActive(true);

            Vector3 normal = (oppPosition - transformPosition).normalized;
            Vector3 targetPosition = transformPosition + normal * 10f;

            // if opp is in range, jump behind him not *10f
            if (IsOpponentInRange())
            {
                targetPosition = transform.position + normal * (Attack.Range * 2);
                StartCoroutine(UnitController.Opponent.GetHit(Attack));
            }

            UnitController.Collider.enabled = false;
            targetPosition.y = 1;
            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.PlaySfx(Attack.Sound, transform.position);
            UnitController.transform.DOJump(targetPosition, 2f, 1, 0.3f)
                .OnComplete(() => UnitController.Collider.enabled = true);

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}