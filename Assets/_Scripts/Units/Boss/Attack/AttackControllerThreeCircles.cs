using System.Collections;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Boss.Attack
{
    public class AttackControllerThreeCircles : AttackController
    {
        public override IEnumerator Execute()
        {
            int total = Random.Range(50, 80);
            int shotsPerGroup = total / 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < shotsPerGroup; j++)
                {
                    Vector3 spawnPos = transform.position;
                    spawnPos.y = 1f;
                    float startAngle = 0;
                    if (i == 1) startAngle = 10;
                    if (i == 2) startAngle = -10;
                    Vector3 pos = Helpers.GetPositionOnCircle(transform.position, 5, j, shotsPerGroup, startAngle);
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