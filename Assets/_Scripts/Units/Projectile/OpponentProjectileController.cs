using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileController : ProjectileController
    {
        [SerializeField] Attack.Attack _attack;

        public virtual void Shoot(Vector3 dir, float time)
        {
            Time = time;
            Direction = dir;

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(Direction));
        }

        protected override void HitTarget(UnitController target)
        {
            StartCoroutine(target.GetHit(_attack));
            HitConnected();
        }
    }
}