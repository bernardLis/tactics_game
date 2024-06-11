using System.Collections;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Boss
{
    public class AttackControllerCircle : AttackController
    {
        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
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