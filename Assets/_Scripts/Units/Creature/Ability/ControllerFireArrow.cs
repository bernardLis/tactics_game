using System.Collections;
using DG.Tweening;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerFireArrow : Controller
    {
        [SerializeField] GameObject _abilityProjectile;
        ProjectileController _projectileController;

        public override void Initialize(CreatureController creatureController)
        {
            base.Initialize(creatureController);
            _projectileController = Instantiate(_abilityProjectile, BattleManager.EntityHolder).GetComponent<ProjectileController>();
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!UnitAttackController.IsOpponentInRange()) yield break;

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySfx(Ability.Sound, transform.position);

            Vector3 oppPos = CreatureController.Opponent.transform.position;
            yield return transform.DODynamicLookAt(oppPos, 0.2f).WaitForCompletion();

            CreatureControllerRanged bcr = (CreatureControllerRanged)CreatureController;
            _projectileController.transform.position = bcr.ProjectileSpawnPoint.transform.position;
            _projectileController.Initialize(bcr.Team);
            Vector3 dir = (oppPos - transform.position).normalized;
            _projectileController.Shoot(bcr, dir);
            yield return base.ExecuteAbilityCoroutine();
        }
    }
}