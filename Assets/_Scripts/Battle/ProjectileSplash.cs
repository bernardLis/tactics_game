using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSplash : Projectile
{
    [SerializeField] float _splashRadius = 3f;
    protected override IEnumerator HitTarget(BattleEntity target)
    {
        Collider[] colliders = Physics.OverlapSphere(target.Collider.bounds.center, _splashRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<BattleEntity>(out BattleEntity entity))
            {
                if (entity.Team == _shooterCreature.Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(_shooterCreature));
            }
        }
        yield return DestroySelf(transform.position);
    }

}
