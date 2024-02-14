using System;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Building")]
    public class Building : BaseScriptableObject
    {
        public Sprite Icon;
        [HideInInspector] public Upgrade BuildingUpgrade;

        public event Action OnUnlocked;

        public GameObject BuildingPrefab;

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

        public virtual void Unlocked()
        {
            OnUnlocked?.Invoke();
        }
    }
}