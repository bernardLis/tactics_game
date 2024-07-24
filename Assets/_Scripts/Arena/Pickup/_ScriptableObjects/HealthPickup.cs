using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Health")]
    public class HealthPickup : Pickup
    {
        public override void Collected(Hero hero)
        {
            HeroManager.Instance.HeroController.GetHealed(50);
        }
    }
}