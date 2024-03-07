using Lis.Battle.Pickup;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileControllerWallBounce : OpponentProjectileController
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Tags.UnpassableLayer ||
                collision.gameObject.layer == Tags.BattleFloorLayer ||
                collision.gameObject.layer == Tags.BattleInteractableLayer)
            {
                Bounce(collision.contacts[0].normal);
                return;
            }

            if (IsHitConnected) return;
            if (collision.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (!IsTargetValid(battleEntity)) return;

                IsHitConnected = true;
                StopAllCoroutines();
                HitTarget(battleEntity);
            }

            if (collision.gameObject.TryGetComponent(out BreakableVaseController bbv))
            {
                HitConnected();
                bbv.TriggerBreak();
            }
        }

        void Bounce(Vector3 normal)
        {
            Direction = Vector3.Reflect(Direction, normal);
            Direction.y = 0;
            Direction.Normalize();
        }
    }
}