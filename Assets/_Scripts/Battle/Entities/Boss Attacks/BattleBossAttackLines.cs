using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackLines : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty
        int shotsPerGroup = total / _attack.GroupCount;

        for (int i = 0; i < _attack.GroupCount; i++)
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
