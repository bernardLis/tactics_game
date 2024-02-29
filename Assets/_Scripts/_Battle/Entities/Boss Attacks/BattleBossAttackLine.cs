using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackLine : BattleBossAttack
    {

        public override IEnumerator Attack()
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y);
            int shotsPerGroup = total / BossAttack.GroupCount;
            float waitTime = BossAttack.TotalAttackDuration / BossAttack.GroupCount;

            for (int i = 0; i < BossAttack.GroupCount; i++)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y = 1f;
                Vector3 pos = BattleManager.BattleHero.transform.position;
                pos.y = 1f;
                Vector3 dir = (pos - spawnPos).normalized;
                for (int j = 0; j < shotsPerGroup; j++)
                {
                    SpawnProjectile(dir);
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
}
