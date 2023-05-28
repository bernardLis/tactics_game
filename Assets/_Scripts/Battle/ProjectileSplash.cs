using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSplash : Projectile
{
    [SerializeField] float _splashRadius = 3f;
    protected override IEnumerator HitTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(_target.Collider.bounds.center, _splashRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<BattleEntity>(out BattleEntity entity))
                StartCoroutine(entity.GetHit(_shooter));
        }
        yield return null;
    }

}
