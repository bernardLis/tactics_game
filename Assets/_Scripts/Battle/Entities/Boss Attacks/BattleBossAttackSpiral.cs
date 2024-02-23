using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackSpiral : BattleBossAttack
    {

        public override IEnumerator Attack()
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y);
            int shotsPerGroup = total / BossAttack.GroupCount;
            float waitTime = 3f / BossAttack.GroupCount;
            for (int i = 0; i < BossAttack.GroupCount; i++)
            {
                for (int j = 0; j < shotsPerGroup; j++)
                {
                    Vector3 dir = Quaternion.Euler(0, i * BossAttack.Spread + j * 90, 0) * Vector3.forward;
                    SpawnProjectile(dir);
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
