
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Bag")]
    public class Bag : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<BattlePickupManager>().BagCollected();
        }
    }
}
