using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackRandom : BattleBossAttack
    {

        public override IEnumerator Attack()
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y);

            float waitTime = BossAttack.TotalAttackDuration / total;
            for (int i = 0; i < total; i++)
            {
                Vector3 dir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
                SpawnProjectile(dir);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
