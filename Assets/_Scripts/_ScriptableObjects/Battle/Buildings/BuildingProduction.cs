using Lis.Upgrades;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building Production")]
    public class BuildingProduction : Building
    {
        public Creature ProducedCreature;

        public UpgradeLevelLair GetCurrentUpgrade()
        {
            return BuildingUpgrade.GetCurrentLevel() as UpgradeLevelLair;
        }
    }
}