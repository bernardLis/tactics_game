using System.Collections;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units
{
    [RequireComponent(typeof(ProjectilePoolManager))]
    public class UnitAttackControllerRanged : UnitAttackController
    {
        public GameObject ProjectileSpawnPoint;
        ProjectilePoolManager _projectilePoolManager;

        public override void Initialize(UnitController unitController)
        {
            base.Initialize(unitController);
            _projectilePoolManager = GetComponent<ProjectilePoolManager>();
            Creature.Creature c = (Creature.Creature)unitController.Unit;
            _projectilePoolManager.Initialize(c.Projectile);
        }

        public override IEnumerator AttackCoroutine()
        {
            yield return BaseAttackCoroutine();
            ProjectileController projectileController = _projectilePoolManager.GetObjectFromPool();
            projectileController.Initialize(UnitController.Team);
            projectileController.transform.position = ProjectileSpawnPoint.transform.position;
            if (UnitController.Opponent == null) yield break;
            Vector3 dir = (UnitController.Opponent.transform.position - transform.position).normalized;
            dir.y = 0;

            projectileController.Shoot(UnitController, dir);
        }
    }
}