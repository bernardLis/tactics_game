using System.Collections;
using UnityEngine;

namespace Lis.Units.Boss
{
    public class AttackControllerSpiral : AttackController
    {
        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
            int shotsPerGroup = total / Attack.GroupCount;
            float waitTime = 3f / Attack.GroupCount;
            for (int i = 0; i < Attack.GroupCount; i++)
            {
                for (int j = 0; j < shotsPerGroup; j++)
                {
                    Vector3 dir = Quaternion.Euler(0, i * Attack.Spread + j * 90, 0) * Vector3.forward;
                    SpawnProjectile(dir);
                }

                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}