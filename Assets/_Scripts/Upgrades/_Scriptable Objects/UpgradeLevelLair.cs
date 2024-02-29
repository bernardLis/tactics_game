using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Level Lair")]
    public class UpgradeLevelLair : UpgradeLevel
    {
        public float ProductionDelay;
        public int ProductionLimit;

    }
}
