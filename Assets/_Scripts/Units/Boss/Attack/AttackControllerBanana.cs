using System.Collections;
using UnityEngine;

namespace Lis.Units.Boss
{
    public class AttackControllerBanana : AttackController
    {
        public override IEnumerator Execute()
        {
            int total = Random.Range(Attack.TotalShotCount.x, Attack.TotalShotCount.y);
            int shotsPerGroup = total / Attack.GroupCount;
            float waitTime = Attack.TotalAttackDuration / Attack.GroupCount;
            int halfTotalShots = Mathf.FloorToInt(shotsPerGroup / 2);

            for (int i = 0; i < Attack.GroupCount; i++)
            {
                Vector3 heroPosNorm = HeroController.transform.position;
                heroPosNorm.y = 1f;
                Vector3 transformPosNorm = new(transform.position.x, 1f, transform.position.z);

                for (int j = -halfTotalShots; j <= halfTotalShots; j++)
                {
                    Vector3 dirToHero = (heroPosNorm - transformPosNorm).normalized;
                    dirToHero += Quaternion.Euler(0, j * Attack.Spread, 0) * dirToHero;
                    dirToHero = dirToHero.normalized;

                    SpawnProjectile(dirToHero);
                }

                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }
}