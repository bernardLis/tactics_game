using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProjectileSplash : BattleProjectile
{
    [SerializeField] float _splashRadius = 3f;
    protected override IEnumerator HitTarget(BattleEntity target)
    {
        Collider[] colliders = Physics.OverlapSphere(target.Collider.bounds.center, _splashRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == _shooter.Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(_shooter));
            }
        }
        yield return Explode(transform.position);
    }

}
