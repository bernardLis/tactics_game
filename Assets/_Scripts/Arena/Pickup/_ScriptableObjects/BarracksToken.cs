using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/BarracksToken")]
    public class BarracksToken : Pickup
    {
        public override void Collected(Hero hero)
        {
            FightManager.Instance.Campaign.Barracks.ActivateUpgradeToken();
        }
    }
}