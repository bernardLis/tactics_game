
using UnityEngine;

namespace Lis
{
    public class BattleProjectileSplash : BattleProjectile
    {
        [SerializeField] float _splashRadius = 3f;
        protected override void HitTarget(BattleEntity target)
        {
            Collider[] colliders = Physics.OverlapSphere(target.Collider.bounds.center, _splashRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out BattleEntity entity))
                {
                    if (entity.Team == Shooter.Team) continue; // splash damage is player friendly
                    if (entity.IsDead) continue;

                    StartCoroutine(entity.GetHit(Shooter));
                }
            }
            StartCoroutine(Explode(transform.position));
        }

    }
}
