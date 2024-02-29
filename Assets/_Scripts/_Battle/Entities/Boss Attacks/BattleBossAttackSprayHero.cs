using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackSprayHero : BattleBossAttack
    {
        public override IEnumerator Attack()
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y);
            float waitTime = BossAttack.TotalAttackDuration / total;
            for (int i = 0; i < total; i++)
            {
                Vector3 heroPosAdjusted = BattleManager.BattleHero.transform.position
                                          + new Vector3(Random.Range(-BossAttack.Spread, BossAttack.Spread),
                                              0,
                                              Random.Range(-BossAttack.Spread, BossAttack.Spread));
                heroPosAdjusted.y = 1f;
                Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);
                Vector3 dir = heroPosAdjusted - transformPosNorm;
                dir = dir.normalized;
                SpawnProjectile(dir);
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }
}