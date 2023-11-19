using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackLineShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty
        int shotsPerGroup = total / _attack.GroupCount;
        float waitTime = _attack.TotalAttackDuration / _attack.GroupCount;

        for (int i = 0; i < _attack.GroupCount; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            Vector3 pos = _battleManager.BattleHero.transform.position;
            pos.y = 1f;
            Vector3 dir = (pos - spawnPos).normalized;
            for (int j = 0; j < shotsPerGroup; j++)
            {
                SpawnProjectile(dir);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
