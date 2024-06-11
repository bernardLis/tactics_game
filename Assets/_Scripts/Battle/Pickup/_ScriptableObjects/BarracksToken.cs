using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickups/BarracksToken")]
    public class BarracksToken : Pickup
    {
        public override void Collected(Hero hero)
        {
            BattleManager.Instance.Battle.Barracks.ActivateUpgradeToken();
        }
    }
}