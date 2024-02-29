using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Bag")]
    public class Bag : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<PickupManager>().BagCollected();
        }
    }
}
