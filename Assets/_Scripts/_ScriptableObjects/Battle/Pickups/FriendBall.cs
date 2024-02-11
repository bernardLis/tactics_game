using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Friend Ball")]
    public class FriendBall : Pickup
    {
        public override void Collected(Hero hero)
        {
            hero.AddFriendBalls(1);
        }
    }
}