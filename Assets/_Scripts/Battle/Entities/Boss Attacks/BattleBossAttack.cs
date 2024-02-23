using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttack : MonoBehaviour
    {
        protected BattleManager BattleManager;
        BattleRangedOpponentManager _battleRangedOpponentManager;

        protected BossAttack BossAttack;
        BattleBoss _battleBoss;

        public void Initialize(BossAttack bossAttack, BattleBoss battleBoss)
        {
            BossAttack = bossAttack;
            _battleBoss = battleBoss;

            BattleManager = BattleManager.Instance;

            _battleRangedOpponentManager = BattleManager.GetComponent<BattleRangedOpponentManager>();
        }

        public virtual IEnumerator Attack(int difficulty)
        {
            // Meant to be overwritten
            yield return new WaitForSeconds(1f);
        }

        protected void SpawnProjectile(Vector3 dir)
        {
            // basic projectile
            BattleProjectileOpponent p = _battleRangedOpponentManager.GetProjectileFromPool(ElementName.Water);

            if (BossAttack.UseElementalProjectile)
                p = _battleRangedOpponentManager.GetProjectileFromPool(_battleBoss.Entity.Element.ElementName);

            Vector3 spawnPos = transform.position;
            spawnPos.y = 0.5f;

            p.transform.position = spawnPos;
            p.Initialize(1);
            p.Shoot(_battleBoss, dir, BossAttack.ProjectileDuration, BossAttack.ProjectilePower);
        }
    }
}