using System.Collections;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class WildboltProjectileController : ProjectileController
    {
        Vector3 _dir;

        public void Fire(Ability ability)
        {
            Ability = ability;
            EnableProjectile();
            StartCoroutine(FireCoroutine());
        }

        IEnumerator FireCoroutine()
        {
            float endTime = UnityEngine.Time.time + Ability.GetDuration();
            while (UnityEngine.Time.time < endTime)
            {
                // choose a random direction but keep y = 0
                _dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                transform.rotation = Quaternion.LookRotation(_dir);

                Speed = Random.Range(5f, 15f);
                yield return GoForward(Random.Range(0.5f, 2f));
            }

            yield return Explode(transform.position);
        }

        IEnumerator GoForward(float timeInSeconds)
        {
            float t = 0;
            while (t <= timeInSeconds)
            {
                transform.position += Speed * UnityEngine.Time.fixedDeltaTime * _dir;
                t += UnityEngine.Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        protected override void CollideWithUnpassable(Collision collision)
        {
            _dir = Vector3.Reflect(_dir, collision.contacts[0].normal);
            transform.rotation = Quaternion.LookRotation(_dir);
        }
    }
}