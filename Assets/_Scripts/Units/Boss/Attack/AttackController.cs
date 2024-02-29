using System.Collections;
using Lis.Core;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackController : MonoBehaviour
    {
        protected BattleManager BattleManager;
        BattleRangedOpponentManager _battleRangedOpponentManager;

        protected Attack Attack;
        BossController _bossController;

        public void Initialize(Attack attack, BossController bossController)
        {
            Attack = attack;
            _bossController = bossController;

            BattleManager = BattleManager.Instance;

            _battleRangedOpponentManager = BattleManager.GetComponent<BattleRangedOpponentManager>();
        }

        public virtual IEnumerator Execute()
        {
            // Meant to be overwritten
            yield return new WaitForSeconds(1f);
        }

        protected void SpawnProjectile(Vector3 dir)
        {
            // basic projectile
            OpponentProjectileController p = _battleRangedOpponentManager.GetProjectileFromPool(ElementName.Water);

            if (Attack.UseElementalProjectile)
                p = _battleRangedOpponentManager.GetProjectileFromPool(_bossController.Unit.Element.ElementName);

            Vector3 spawnPos = transform.position;
            spawnPos.y = 0.5f;

            p.transform.position = spawnPos;
            p.Initialize(1);
            p.Shoot(_bossController, dir, Attack.ProjectileDuration, Attack.ProjectilePower);
        }
    }
}