using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Mushroom")]
    public class Mushroom : Pickup
    {
        public override void Collected(Hero hero)
        {
            hero.AddExp(hero.ExpForNextLevel.Value);
        }
    }
}