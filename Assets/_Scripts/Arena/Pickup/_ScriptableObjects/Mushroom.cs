using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Mushroom")]
    public class Mushroom : Pickup
    {
        public override void Collected(Hero hero)
        {
            hero.AddExp(hero.ExpForNextLevel.Value);
        }
    }
}