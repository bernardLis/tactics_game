
using UnityEngine;

namespace Lis
{
    public class BattleProjectileOpponent : BattleProjectile
    {
        BattleEntity _battleEntity;
        int _power;

        protected Vector3 Direction;

        public virtual void Shoot(BattleEntity shooter, Vector3 dir, float time, int power)
        {
            _battleEntity = shooter;
            Time = time;
            _power = power;
            Direction = dir;

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(Direction));
        }

        protected override void HitTarget(BattleEntity target)
        {
            target.BaseGetHit(_power, Color.black);
            HitConnected();
        }

    }
}
