using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleProjectileOpponentWallBounce : BattleProjectileOpponent
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
            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (!IsTargetValid(battleEntity)) return;

                IsHitConnected = true;
                StopAllCoroutines();
                HitTarget(battleEntity);
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