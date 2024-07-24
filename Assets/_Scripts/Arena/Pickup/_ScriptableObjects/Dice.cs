using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Dice")]
    public class Dice : Pickup
    {
        public override void Collected(Hero hero)
        {
            HeroManager.Instance.DiceCollected();
        }
    }
}