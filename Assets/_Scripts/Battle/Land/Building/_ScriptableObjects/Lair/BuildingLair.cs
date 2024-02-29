using Lis.Units.Creature;
using Lis.Upgrades;
using UnityEngine;

namespace Lis.Battle.Land.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building Production")]
    public class BuildingLair : Building
    {
        public Creature ProducedCreature;

        public UpgradeLevelLair GetCurrentUpgrade()
        {
            return BuildingUpgrade.GetCurrentLevel() as UpgradeLevelLair;
        }
    }
}