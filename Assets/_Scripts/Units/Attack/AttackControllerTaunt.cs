using System.Collections;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerTaunt : AttackController
    {
        readonly float _radius = 5f;
        [SerializeField] GameObject _effect;

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.PlaySfx(Attack.Sound, transform.position);
            _effect.SetActive(true);
            foreach (UnitController uc in GetOpponentsInRadius(_radius))
            {
                uc.DisplayFloatingText("Taunted", Color.red);
                uc.GetEngaged(UnitController);
                StartCoroutine(uc.GetHit(Attack));
            }

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}