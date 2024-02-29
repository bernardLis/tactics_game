using System.Collections;


using UnityEngine;

namespace Lis
{
    public class BattleWildbolt : BattleProjectile
    {
        public void Fire(Ability ability)
        {
            Ability = ability;
            EnableProjectile();
            StartCoroutine(FireCoroutine());
        }

        public IEnumerator FireCoroutine()
        {
            float endTime = UnityEngine.Time.time + Ability.GetDuration();
            while (UnityEngine.Time.time < endTime)
            {
                // choose a random direction but keep y = 0
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                transform.rotation = Quaternion.LookRotation(dir);

                Speed = Random.Range(5f, 15f);
                yield return GoForward(dir, Random.Range(0.5f, 2f));
            }
            yield return Explode(transform.position);
        }

        IEnumerator GoForward(Vector3 dir, float timeInSeconds)
        {
            float t = 0;
            while (t <= timeInSeconds)
            {
                transform.position += Speed * UnityEngine.Time.fixedDeltaTime * dir;
                t += UnityEngine.Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
