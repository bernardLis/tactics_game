using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class BattleProjectileOpponentWallBounce : BattleProjectileOpponent
{

    protected override void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == Tags.BattleObstacleLayer ||
            collision.gameObject.layer == Tags.BattleFloorLayer ||
            collision.gameObject.layer == Tags.BattleInteractableLayer)
        {

            Bounce(collision.contacts[0].normal);
            return;
        }

        if (_hitConnected) return;
        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (!IsTargetValid(battleEntity)) return;

            _hitConnected = true;
            StopAllCoroutines();
            StartCoroutine(HitTarget(battleEntity));
            return;
        }
    }

    void Bounce(Vector3 normal)
    {
        Debug.Log($"bounce");
        _direction = Vector3.Reflect(_direction, normal);
        _direction.y = 0;
        _direction.Normalize();
    }
}
