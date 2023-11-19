using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackSpiral : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty
        int shotsPerGroup = total / _attack.GroupCount;
        float waitTime = 3f / _attack.GroupCount;
        for (int i = 0; i < _attack.GroupCount; i++)
        {
            for (int j = 0; j < shotsPerGroup; j++)
            {
                Vector3 dir = Quaternion.Euler(0, i * _attack.Spread + j * 90, 0) * Vector3.forward;
                SpawnProjectile(dir);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
