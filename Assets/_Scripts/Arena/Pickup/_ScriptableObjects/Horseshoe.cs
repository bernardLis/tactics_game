using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Horseshoe")]
    public class Horseshoe : Pickup
    {
        public override void Collected(Hero hero)
        {
            FightManager.Instance.GetComponent<PickupManager>().HorseshoeCollected();
        }
    }
}