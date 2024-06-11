using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerPoisonBite : AttackController
    {
        [SerializeField] GameObject _effect;

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;
            BaseAttack();

            yield return UnitController.transform.DODynamicLookAt(UnitController.Opponent.transform.position,
                0.2f, AxisConstraint.Y);
            _effect.transform.position = UnitController.Opponent.transform.position;
            _effect.SetActive(true);

            yield return UnitController.Opponent.GetHit(Attack);
            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}