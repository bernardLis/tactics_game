using System.Collections;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackControllerLines : AttackController
    {

        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
            int shotsPerGroup = total / Attack.GroupCount;

            for (int i = 0; i < Attack.GroupCount; i++)
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
