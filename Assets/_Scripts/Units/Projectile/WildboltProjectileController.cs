using System.Collections;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class WildboltProjectileController : ProjectileController
    {
        private Ability _ability;
        private Vector3 _dir;

        public void Fire(Ability ability)
        {
            _ability = ability;
            EnableProjectile();
            StartCoroutine(FireCoroutine());
        }

        private IEnumerator FireCoroutine()
        {
            float endTime = Time.time + _ability.GetDuration();
            while (Time.time < endTime)
            {
                // choose a random direction but keep y = 0
                _dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                transform.rotation = Quaternion.LookRotation(_dir);

                Speed = Random.Range(5f, 15f);
                yield return GoForward(Random.Range(0.5f, 2f));
            }

            yield return Explode(transform.position);
        }

        private IEnumerator GoForward(float timeInSeconds)
        {
            float t = 0;
            while (t <= timeInSeconds)
            {
                transform.position += Speed * Time.fixedDeltaTime * _dir;
                t += Time.fixedDeltaTime;
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