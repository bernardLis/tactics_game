using System.Collections;
using Lis.Units.Attack;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units
{
    public class UnitAttackControllerRanged : UnitAttackController
    {
        public GameObject ProjectileSpawnPoint;

        public override IEnumerator AttackCoroutine()
        {
            yield return BaseAttackCoroutine();
            AttackRanged attackRanged = (AttackRanged)UnitController.Unit.CurrentAttack;
            ProjectileController projectileController =
                Instantiate(attackRanged.ProjectilePrefab).GetComponent<ProjectileController>();
            projectileController.Initialize(UnitController.Team);
            projectileController.transform.position = ProjectileSpawnPoint.transform.position;
            if (UnitController.Opponent == null) yield break;
            Vector3 dir = (UnitController.Opponent.transform.position - transform.position).normalized;
            dir.y = 0;

            projectileController.Shoot(UnitController, dir);
        }
    }
}