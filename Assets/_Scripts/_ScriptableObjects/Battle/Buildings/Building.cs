using System;


using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building")]
    public class Building : BaseScriptableObject
    {
        public Sprite Icon;
        public Upgrade BuildingUpgrade;

        public int SecondsToCorrupt;

        public event Action OnUnlocked;

        public GameObject BuildingPrefab;
        public GameObject TileIndicatorPrefab;
        
        public virtual void Initialize()
        {
            BuildingUpgrade = GameManager.Instance.UpgradeBoard
                .GetUpgradeByName(name);
            BuildingUpgrade.DebugInitialize();
        }

        public bool IsUnlocked()
        {
            return BuildingUpgrade.CurrentLevel >= 0;
        }

        public void Unlocked()
        {
            OnUnlocked?.Invoke();
        }
    }
}
