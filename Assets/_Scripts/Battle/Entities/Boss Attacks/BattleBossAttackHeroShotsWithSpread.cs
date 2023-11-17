using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackHeroShotsWithSpread : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty
        float spreadMultiplier = 10;
        float waitTime = 2f / total;
        for (int i = 0; i < total; i++)
        {
            Vector3 heroPosAdjusted = _battleManager.BattleHero.transform.position
                                    + new Vector3(Random.Range(-spreadMultiplier, spreadMultiplier),
                                     0,
                                     Random.Range(-spreadMultiplier, spreadMultiplier));
            heroPosAdjusted.y = 1f;
            Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);
            Vector3 dir = heroPosAdjusted - transformPosNorm;
            dir = dir.normalized;
            SpawnProjectile(dir, 10f, 1);
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
