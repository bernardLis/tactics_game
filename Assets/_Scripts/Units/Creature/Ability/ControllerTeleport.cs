using System.Collections;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerTeleport : Controller
    {
        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        public override void Initialize(CreatureController creatureController)
        {
            base.Initialize(creatureController);
            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity, BattleManager.EntityHolder);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);
            CreatureControllerRanged bca = (CreatureControllerRanged)CreatureController;
            Vector3 point = bca.ClosestPositionWithClearLos();
            bca.transform.position = point;

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            if (_effectInstance != null)
                _effectInstance.SetActive(false);
        }
    }
}