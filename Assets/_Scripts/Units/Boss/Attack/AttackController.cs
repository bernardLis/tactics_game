using System.Collections;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackController : MonoBehaviour
    {
        protected BattleManager BattleManager;
        RangedOpponentManager _rangedOpponentManager;

        protected Attack Attack;
        BossController _bossController;

        protected HeroController HeroController;

        public void Initialize(Attack attack, BossController bossController)
        {
            Attack = attack;
            _bossController = bossController;

            BattleManager = BattleManager.Instance;

            HeroController = BattleManager.GetComponent<HeroManager>().HeroController;

            _rangedOpponentManager = BattleManager.GetComponent<RangedOpponentManager>();
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
            p.Initialize(1);
            p.Shoot(_bossController, dir, Attack.ProjectileDuration, Attack.ProjectilePower);
        }
    }
}