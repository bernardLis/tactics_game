using System.Collections;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackControllerLine : AttackController
    {

        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
            int shotsPerGroup = total / Attack.GroupCount;
            float waitTime = Attack.TotalAttackDuration / Attack.GroupCount;

            for (int i = 0; i < Attack.GroupCount; i++)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y = 1f;
                Vector3 pos = HeroController.transform.position;
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
