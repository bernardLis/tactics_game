using Lis.Arena.Fight;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Bag")]
    public class Bag : Pickup
    {
        public override void Collected(Hero hero)
        {
            FightManager.Instance.GetComponent<PickupManager>().BagCollected();
        }
    }
}