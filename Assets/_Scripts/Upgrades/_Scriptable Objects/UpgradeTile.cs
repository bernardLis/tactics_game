using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Tile")]
    public class UpgradeTile : Upgrade
    {
        public GameObject TilePrefab;
    }
}