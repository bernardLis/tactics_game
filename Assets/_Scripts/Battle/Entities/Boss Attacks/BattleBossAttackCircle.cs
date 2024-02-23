using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackCircle : BattleBossAttack
    {

        public override IEnumerator Attack(int difficulty)
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y); // TODO: difficulty
            for (int i = 0; i < total; i++)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y = 1f;
                Vector3 pos = Helpers.GetPositionOnCircle(transform.position, 5, i, total);
                pos.y = 1f;
                Vector3 dir = (pos - spawnPos).normalized;
                SpawnProjectile(dir);
            }
            yield return null;
        }
    }
}
