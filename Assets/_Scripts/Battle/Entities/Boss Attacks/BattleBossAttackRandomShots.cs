using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackRandomShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty

        float waitTime = 3f / total;
        for (int i = 0; i < total; i++)
        {
            Vector3 dir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
            SpawnProjectile(dir, 10f, 5);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
