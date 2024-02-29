
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileController : ProjectileController
    {
        UnitController _unitController;
        int _power;


        public virtual void Shoot(UnitController shooter, Vector3 dir, float time, int power)
        {
            _unitController = shooter;
            Time = time;
            _power = power;
            Direction = dir;

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(Direction));
        }

        protected override void HitTarget(UnitController target)
        {
            target.BaseGetHit(_power, Color.black);
            HitConnected();
        }

    }
}
