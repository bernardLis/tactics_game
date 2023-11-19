using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackRandomShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty

        float waitTime = _attack.TotalAttackDuration / total;
        for (int i = 0; i < total; i++)
        {
            Vector3 dir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
            SpawnProjectile(dir);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
