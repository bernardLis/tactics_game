using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackCircleShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty
        for (int i = 0; i < total; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            Vector3 pos = Helpers.GetPositionOnCircle(transform.position, 5, i, total);
            pos.y = 1f;
            Vector3 dir = (pos - spawnPos).normalized;
            SpawnProjectile(dir, 10f, 5);
        }
        yield return null;
    }
}
