using Lis.Battle.Land.Building;
using Lis.Core;
using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Building")]
    public class UpgradeBuilding : Upgrade
    {
        public GameObject BuildingPrefab;
    }
}