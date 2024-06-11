using System.Collections;
using System.Collections.Generic;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class FireballsController : Controller
    {
        [SerializeField] private GameObject _fireballPrefab;
        private readonly List<ProjectileController> _fireballPool = new();

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0f, 0.5f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            Vector3 dir = GetRandomEnemyDirection();
            int numberOfLines = 50;
            //            Vector3 dir = transform.position - _target.transform.position;

            //
            //rotatedLine = rotatedLine.normalized * _attackRange;

            // Vector3 projectileVariance = new(0.2f, 0, 0.2f);
            int half = Mathf.FloorToInt(Ability.GetAmount() * 0.5f); // TODO: only "works" for odd numbers
            for (int i = -half; i <= half; i++)
            {
                ProjectileController projectileController = GetInactiveFireball();
                Transform t = projectileController.transform;
                t.localScale = Vector3.one * Ability.GetScale();
                Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
                Vector3 direction = rotatedLine.normalized;
                // t.position = transform.position + projectileVariance * i;
                // projectileController.Shoot(dir + projectileVariance * i, 5);
                t.position = transform.position + direction;
                projectileController.Shoot(direction, 5);
            }
        }

        private ProjectileController GetInactiveFireball()
        {
            foreach (ProjectileController ball in _fireballPool)
                if (!ball.gameObject.activeSelf)
                    return ball;
            return InitializeFireball();
        }

        private ProjectileController InitializeFireball()
        {
            GameObject instance = Instantiate(_fireballPrefab, Vector3.zero, Quaternion.identity,
                BattleManager.AbilityHolder);
            instance.SetActive(true);

            ProjectileController projectileController = instance.GetComponent<ProjectileController>();
            projectileController.Initialize(0, Ability.GetCurrentLevel());
            _fireballPool.Add(projectileController);
            return projectileController;
        }
    }
}