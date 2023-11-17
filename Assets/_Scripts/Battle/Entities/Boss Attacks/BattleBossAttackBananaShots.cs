using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttackBananaShots : BattleBossAttack
{

    public override IEnumerator Attack(int difficulty)
    {
        int total = Random.Range(20, 50); // TODO: difficulty
        int shotsPerGroup = 7;
        int numberOfGroups = total / shotsPerGroup;
        float waitTime = 10f / numberOfGroups;
        float spread = 15;
        int halfTotalShots = Mathf.FloorToInt(shotsPerGroup / 2);

        for (int i = 0; i < numberOfGroups; i++)
        {
            Vector3 heroPosNorm = _battleManager.BattleHero.transform.position;
            heroPosNorm.y = 1f;
            Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);

            for (int j = -halfTotalShots; j <= halfTotalShots; j++)
            {
                Vector3 dirToHero = (heroPosNorm - transformPosNorm).normalized;
                dirToHero += Quaternion.Euler(0, j * spread, 0) * dirToHero;
                dirToHero = dirToHero.normalized;

                SpawnProjectile(dirToHero, 10f, 5);
            }
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
