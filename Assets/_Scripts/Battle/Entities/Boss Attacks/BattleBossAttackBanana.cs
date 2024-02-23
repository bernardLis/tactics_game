using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackBanana : BattleBossAttack
    {

        public override IEnumerator Attack(int difficulty)
        {
            int total = Random.Range(BossAttack.TotalShotCount.x, BossAttack.TotalShotCount.y); // TODO: difficulty
            int shotsPerGroup = total / BossAttack.GroupCount;
            float waitTime = BossAttack.TotalAttackDuration / BossAttack.GroupCount;
            int halfTotalShots = Mathf.FloorToInt(shotsPerGroup / 2);

            for (int i = 0; i < BossAttack.GroupCount; i++)
            {
                Vector3 heroPosNorm = BattleManager.BattleHero.transform.position;
                heroPosNorm.y = 1f;
                Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);

                for (int j = -halfTotalShots; j <= halfTotalShots; j++)
                {
                    Vector3 dirToHero = (heroPosNorm - transformPosNorm).normalized;
                    dirToHero += Quaternion.Euler(0, j * BossAttack.Spread, 0) * dirToHero;
                    dirToHero = dirToHero.normalized;

                    SpawnProjectile(dirToHero);
                }
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
}
