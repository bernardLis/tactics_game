using System.Collections;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackLines : BattleBossAttack
    {

        public override IEnumerator Attack()
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y);
            int shotsPerGroup = total / BossAttack.GroupCount;

            for (int i = 0; i < BossAttack.GroupCount; i++)
            {
                for (int j = 0; j < shotsPerGroup; j++)
                {
                    Vector3 spawnPos = transform.position;
                    spawnPos.y = 1f;
                    Vector3 pos = Helpers.GetPositionOnCircle(transform.position, 5 - i, j, shotsPerGroup);
                    pos.y = 1f;
                    Vector3 dir = (pos - spawnPos).normalized;
                    SpawnProjectile(dir);
                }
                yield return new WaitForSeconds(0.3f);
            }
            yield return null;
        }
    }
}
