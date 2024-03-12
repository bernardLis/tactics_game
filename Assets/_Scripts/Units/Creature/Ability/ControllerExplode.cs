using System.Collections;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerExplode : Controller
    {
        readonly float _explosionRadius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            _effect.SetActive(true);
            foreach (UnitController be in GetOpponentsInRadius(_explosionRadius))
                StartCoroutine(be.GetHit(CreatureController, 50));

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}