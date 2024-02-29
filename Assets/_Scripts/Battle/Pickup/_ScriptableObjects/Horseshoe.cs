using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/Horseshoe")]
    public class Horseshoe : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<PickupManager>().HorseshoeCollected();
        }
    }
}
