using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Barracks/Unlockable Nature")]
    public class UnlockableNature : BaseScriptableObject
    {
        public Nature Nature;
        public bool IsUnlocked;
        public int Price;
        public string Description;
    }
}