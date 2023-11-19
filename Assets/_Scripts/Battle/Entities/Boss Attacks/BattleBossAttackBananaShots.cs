using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackBananaShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty
        int shotsPerGroup = total / _attack.GroupCount;
        float waitTime = _attack.TotalAttackDuration / _attack.GroupCount;
        int halfTotalShots = Mathf.FloorToInt(shotsPerGroup / 2);

        for (int i = 0; i < _attack.GroupCount; i++)
        {
            Vector3 heroPosNorm = _battleManager.BattleHero.transform.position;
            heroPosNorm.y = 1f;
            Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);

            for (int j = -halfTotalShots; j <= halfTotalShots; j++)
            {
                Vector3 dirToHero = (heroPosNorm - transformPosNorm).normalized;
                dirToHero += Quaternion.Euler(0, j * _attack.Spread, 0) * dirToHero;
                dirToHero = dirToHero.normalized;

                SpawnProjectile(dirToHero);
            }
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
