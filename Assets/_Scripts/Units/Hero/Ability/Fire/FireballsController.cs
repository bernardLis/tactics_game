using System.Collections;
using System.Collections.Generic;
using Lis.Units.Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Units.Hero.Ability
{
    public class FireballsController : Controller
    {
        Camera _cam;
        Mouse _mouse;
        [SerializeField] GameObject _fireballPrefab;
        readonly List<ProjectileController> _fireballPool = new();

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            _cam = Camera.main;
            _mouse = Mouse.current;
            transform.localPosition = new(0f, 0.5f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            Vector3 dir = GetRandomEnemyDirection();

            // fly towards mouse if possible
            Ray ray = _cam.ScreenPointToRay(_mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                dir = (hit.point - transform.position).normalized;
            dir.y = 0;

            int numberOfLines = 50;
            int half = Mathf.FloorToInt(Ability.GetAmount() * 0.5f); // TODO: only "works" for odd numbers
            for (int i = -half; i <= half; i++)
            {
                ProjectileController projectileController = GetInactiveFireball();
                Transform t = projectileController.transform;
                t.localScale = Vector3.one * Ability.GetScale();
                Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
                Vector3 direction = rotatedLine.normalized;
                t.position = transform.position + direction;
                projectileController.Shoot(direction, 5);
            }
        }

        ProjectileController GetInactiveFireball()
        {
            foreach (ProjectileController ball in _fireballPool)
                if (!ball.gameObject.activeSelf)
                    return ball;
            return InitializeFireball();
        }

        ProjectileController InitializeFireball()
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