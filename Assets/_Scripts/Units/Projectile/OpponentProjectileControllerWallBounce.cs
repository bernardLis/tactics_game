using Lis.Arena.Pickup;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileControllerWallBounce : OpponentProjectileController
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Tags.UnpassableLayer ||
                collision.gameObject.layer == Tags.ArenaFloorLayer ||
                collision.gameObject.layer == Tags.ArenaInteractableLayer)
            {
                Bounce(collision.contacts[0].normal);
                return;
            }

            if (IsHitConnected) return;
            if (collision.gameObject.TryGetComponent(out UnitController unitController))
            {
                if (!IsTargetValid(unitController)) return;

                IsHitConnected = true;
                HitTarget(unitController);
                HitConnected();
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