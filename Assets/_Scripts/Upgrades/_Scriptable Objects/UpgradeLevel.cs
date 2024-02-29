using Lis.Core;
using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Level")]
    public class UpgradeLevel : BaseScriptableObject
    {
        public int Cost;
        public int Value;
        public string Description;

    }
}
