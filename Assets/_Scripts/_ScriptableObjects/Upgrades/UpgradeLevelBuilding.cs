using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Level Building")]
    public class UpgradeLevelBuilding : UpgradeLevel
    {
        public float ProductionDelay;
        public int ProductionLimit;

    }
}
