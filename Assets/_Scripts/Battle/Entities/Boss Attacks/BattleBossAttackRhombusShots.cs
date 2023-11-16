using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackRhombusShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty
        int numberOfGroups = total / 4;
        float waitTime = 3f / numberOfGroups;
        for (int i = 0; i < numberOfGroups; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 dir = Quaternion.Euler(0, i * 15 + j * 90, 0) * Vector3.forward;
                SpawnProjectile(dir, 10f, 5);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
