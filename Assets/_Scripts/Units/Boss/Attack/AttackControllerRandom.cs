using System.Collections;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackControllerRandom : AttackController
    {

        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);

            float waitTime = Attack.TotalAttackDuration / total;
            for (int i = 0; i < total; i++)
            {
                Vector3 dir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
                SpawnProjectile(dir);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
