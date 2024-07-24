using System.Collections;
using Lis.Arena;
using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Attack;
using Lis.Units.Hero;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Boss
{
    public class AttackController : MonoBehaviour
    {
        BossController _bossController;
        RangedOpponentManager _rangedOpponentManager;

        protected AttackBoss Attack;

        protected HeroController HeroController;

        public void Initialize(AttackBoss attack, BossController bossController)
        {
            Attack = attack;
            _bossController = bossController;

            HeroController = HeroManager.Instance.HeroController;
            _rangedOpponentManager = RangedOpponentManager.Instance;
        }

        public virtual IEnumerator Execute()
        {
            // Meant to be overwritten
            yield return new WaitForSeconds(1f);
        }

        protected void SpawnProjectile(Vector3 dir)
        {
            // basic projectile
            OpponentProjectileController p = _rangedOpponentManager.GetProjectileFromPool(NatureName.Water);

            if (Attack.UseElementalProjectile)
                p = _rangedOpponentManager.GetProjectileFromPool(_bossController.Unit.Nature.NatureName);

            Vector3 spawnPos = transform.position;
            spawnPos.y = 0.5f;

            p.transform.position = spawnPos;
            p.Initialize(1, Attack);
            p.Shoot(dir, Attack.ProjectileDuration);
        }
    }
}