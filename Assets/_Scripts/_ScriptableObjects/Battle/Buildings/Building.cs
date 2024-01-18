using System;


using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building")]
    public class Building : BaseScriptableObject
    {
        public Sprite Icon;
        public bool IsSecured;
        public Upgrade BuildingUpgrade;

        public int SecondsToCorrupt;

        public event Action OnSecured;
        public event Action OnCorrupted;

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

        public void Secure()
        {
            IsSecured = true;
            OnSecured?.Invoke();
        }

        public void Corrupted()
        {
            IsSecured = false;
            OnCorrupted?.Invoke();
        }
    }
}
