using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackLineShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty
        int shotsPerGroup = total / 5;

        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            Vector3 pos = _battleManager.BattleHero.transform.position;
            pos.y = 1f;
            Vector3 dir = (pos - spawnPos).normalized;
            for (int j = 0; j < shotsPerGroup; j++)
            {
                SpawnProjectile(dir, 10f, 5);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
