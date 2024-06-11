using System.Collections;
using System.Collections.Generic;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class AquaJetController : Controller
    {
        [SerializeField] private GameObject _projectilePrefab;
        private readonly List<HomingProjectileController> _projectilePool = new();

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0.5f, 1f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            int projectileCount = Ability.GetAmount();
            for (int i = 0; i < projectileCount; i++)
            {
                Quaternion q = Quaternion.Euler(Random.Range(180f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f));

                HomingProjectileController homingProjectileController = GetInactiveProjectile();
                Transform t = homingProjectileController.transform;
                t.position = transform.position;
                t.rotation = q;

                homingProjectileController.StartHoming(Ability);
                yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
            }
        }

        private HomingProjectileController InitializeProjectile()
        {
            GameObject instance = Instantiate(_projectilePrefab, Vector3.zero, Quaternion.identity,
                BattleManager.AbilityHolder);
            instance.SetActive(true);

            HomingProjectileController homingProjectileController = instance.GetComponent<HomingProjectileController>();
            homingProjectileController.Initialize(0, Ability.GetCurrentLevel());
            _projectilePool.Add(homingProjectileController);
            return homingProjectileController;
        }

        private HomingProjectileController GetInactiveProjectile()
        {
            foreach (HomingProjectileController p in _projectilePool)
                if (!p.gameObject.activeSelf)
                    return p;
            return InitializeProjectile();
        }
    }
}