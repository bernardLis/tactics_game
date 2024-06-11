using System.Collections;
using System.Collections.Generic;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WildboltStormController : Controller
    {
        [SerializeField] private GameObject _effect;

        [SerializeField] private GameObject _wildboltPrefab;
        private readonly List<WildboltProjectileController> _wildboltPool = new();

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0, 0.5f, 0f);
        }


        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            if (Ability.ExecuteSound != null)
                AudioManager.PlaySfx(Ability.ExecuteSound, transform.position);

            Transform t = transform;
            _effect.SetActive(true);
            _effect.transform.parent = t;

            Vector3 pos = t.position;
            pos.y = 0.1f;
            _effect.transform.position = pos;
            yield return new WaitForSeconds(0.6f);
            _effect.transform.parent = BattleManager.AbilityHolder;

            int projectileCount = Ability.GetAmount();
            for (int i = 0; i < projectileCount; i++)
            {
                WildboltProjectileController projectile = GetInactiveProjectile();
                projectile.transform.position = transform.position;
                projectile.Fire(Ability);
            }

            yield return new WaitForSeconds(2f);

            _effect.SetActive(false);
        }

        private WildboltProjectileController InitializeProjectile()
        {
            GameObject instance = Instantiate(_wildboltPrefab, Vector3.zero, Quaternion.identity,
                BattleManager.AbilityHolder);
            instance.SetActive(true);

            WildboltProjectileController projectile = instance.GetComponent<WildboltProjectileController>();
            projectile.Initialize(0, Ability.GetCurrentLevel());
            _wildboltPool.Add(projectile);
            return projectile;
        }

        private WildboltProjectileController GetInactiveProjectile()
        {
            foreach (WildboltProjectileController p in _wildboltPool)
                if (!p.gameObject.activeSelf)
                    return p;
            return InitializeProjectile();
        }
    }
}