using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building Production")]
    public class BuildingProduction : Building
    {
        public Creature ProducedCreature;

        public UpgradeLevelBuilding GetCurrentUpgrade()
        {
            return BuildingUpgrade.GetCurrentLevel() as UpgradeLevelBuilding;
        }
    }
}