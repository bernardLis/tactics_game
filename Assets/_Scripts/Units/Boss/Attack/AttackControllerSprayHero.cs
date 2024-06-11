using System.Collections;
using UnityEngine;

namespace Lis.Units.Boss
{
    public class AttackControllerSprayHero : AttackController
    {
        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
            float waitTime = Attack.TotalAttackDuration / total;
            for (int i = 0; i < total; i++)
            {
                Vector3 heroPosAdjusted = HeroController.transform.position
                                          + new Vector3(Random.Range(-Attack.Spread, Attack.Spread),
                                              0,
                                              Random.Range(-Attack.Spread, Attack.Spread));
                heroPosAdjusted.y = 1f;
                Vector3 transformPosNorm = new(transform.position.x, 1f, transform.position.z);
                Vector3 dir = heroPosAdjusted - transformPosNorm;
                dir = dir.normalized;
                SpawnProjectile(dir);
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }
}