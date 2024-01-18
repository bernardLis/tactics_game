using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBossAttackSprayHero : BattleBossAttack
    {

        public override IEnumerator Attack(int difficulty)
        {
            int total = Random.Range(_attack.TotalShotCount.x, _attack.TotalShotCount.y); // TODO: difficulty
            float waitTime = _attack.TotalAttackDuration / total;
            for (int i = 0; i < total; i++)
            {
                Vector3 heroPosAdjusted = _battleManager.BattleHero.transform.position
                                          + new Vector3(Random.Range(-_attack.Spread, _attack.Spread),
                                              0,
                                              Random.Range(-_attack.Spread, _attack.Spread));
                heroPosAdjusted.y = 1f;
                Vector3 transformPosNorm = new Vector3(transform.position.x, 1f, transform.position.z);
                Vector3 dir = heroPosAdjusted - transformPosNorm;
                dir = dir.normalized;
                SpawnProjectile(dir);
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
}
