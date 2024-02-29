
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Horseshoe")]
    public class Horseshoe : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.GetComponent<BattlePickupManager>().HorseshoeCollected();
        }
    }
}
