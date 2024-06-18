using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Health")]
    public class HealthPickup : Pickup
    {
        public override void Collected(Hero hero)
        {
            HeroManager.Instance.HeroController.GetHealed(50);
        }
    }
}