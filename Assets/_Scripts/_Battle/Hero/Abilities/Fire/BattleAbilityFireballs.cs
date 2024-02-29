using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleAbilityFireballs : BattleAbility
    {
        [SerializeField] GameObject _fireballPrefab;
        readonly List<BattleProjectile> _fireballPool = new();

        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new(0f, 0.5f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            Vector3 dir = GetRandomEnemyDirection();
            Vector3 projectileVariance = new(0.2f, 0, 0.2f);
            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                BattleProjectile projectile = GetInactiveFireball();
                Transform t = projectile.transform;
                t.localScale = Vector3.one * Ability.GetScale();
                t.position = transform.position + projectileVariance * i;
                projectile.Shoot(Ability, dir + projectileVariance * i);
            }
        }

        BattleProjectile GetInactiveFireball()
        {
            foreach (BattleProjectile ball in _fireballPool)
                if (!ball.gameObject.activeSelf)
                    return ball;
            return InitializeFireball();
        }

        BattleProjectile InitializeFireball()
        {
            GameObject instance = Instantiate(_fireballPrefab, Vector3.zero, Quaternion.identity,
                BattleManager.AbilityHolder);
            instance.SetActive(true);

            BattleProjectile projectile = instance.GetComponent<BattleProjectile>();
            projectile.Initialize(0);
            _fireballPool.Add(projectile);
            return projectile;
        }
    }
}