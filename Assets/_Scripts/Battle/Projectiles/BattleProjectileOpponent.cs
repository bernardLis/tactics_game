
using UnityEngine;

namespace Lis
{
    public class BattleProjectileOpponent : BattleProjectile
    {
        BattleEntity _battleEntity;
        int _power;

        protected Vector3 _direction;

        public virtual void Shoot(BattleEntity shooter, Vector3 dir, float time, int power)
        {
            _battleEntity = shooter;
            _time = time;
            _power = power;
            _direction = dir;

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(_direction));
        }

        protected override void HitTarget(BattleEntity target)
        {
            target.BaseGetHit(_power, Color.black);
            HitConnected();
        }

    }
}
