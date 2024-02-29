using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Friend Ball")]
    public class FriendBall : Pickup
    {
        public override void Collected(Hero hero)
        {
            hero.AddFriendBalls(1);
        }
    }
}