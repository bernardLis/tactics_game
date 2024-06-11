using System.Collections;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerRanged : AttackController
    {
        [SerializeField] private GameObject _projectilePrefab;
        private ProjectilePoolManager _projectilePoolManager;

        public override void Initialize(UnitController unitController, Attack attack)
        {
            base.Initialize(unitController, attack);
            _projectilePoolManager = GetComponent<ProjectilePoolManager>();
            _projectilePoolManager.Initialize(_projectilePrefab);
        }

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;

            BaseAttack();
            yield return BasicAttackCoroutine();

            ProjectileController projectileController = _projectilePoolManager.GetObjectFromPool();
            projectileController.Initialize(UnitController.Team, Attack);

            Vector3 pos = UnitController.transform.position;
            pos.y = 1;
            projectileController.transform.position = pos;

            if (UnitController.Opponent == null) yield break;
            Vector3 dir = (UnitController.Opponent.transform.position - transform.position).normalized;
            dir.y = 0;
            projectileController.Shoot(dir, 5);
        }
    }
}