using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Board")]
    public class UpgradeBoard : BaseScriptableObject
    {
        [SerializeField] private List<Upgrade> _upgradeOriginals = new();
        public List<Upgrade> Upgrades = new();

        private Dictionary<string, Upgrade> _upgradeDictionary = new();

        public void Reset()
        {
            Upgrades = new();
            foreach (Upgrade original in _upgradeOriginals)
            {
                Upgrade instance = Instantiate(original);
                instance.name = original.name;
                Upgrades.Add(instance);
            }
        }

        public event Action OnRefundAll;
        public event Action OnUnlockAll;

        public void Initialize()
        {
            _upgradeDictionary = new();
            foreach (Upgrade upgrade in Upgrades)
            {
                upgrade.Initialize(this);
                _upgradeDictionary.Add(upgrade.name, upgrade);
            }
        }

        public Upgrade GetUpgradeByName(string n)
        {
            if (_upgradeDictionary.Count == 0) Initialize();

            return _upgradeDictionary.GetValueOrDefault(n);
        }

        public List<Upgrade> GetUpgradesByType(UpgradeType type)
        {
            List<Upgrade> upgrades = new();
            foreach (Upgrade upgrade in Upgrades)
                if (upgrade.Type == type)
                    upgrades.Add(upgrade);
            return upgrades;
        }

        public List<UpgradeTile> GetUnlockedBuildings()
        {
            List<UpgradeTile> buildings = new();
            foreach (Upgrade upgrade in Upgrades)
                if (upgrade.Type == UpgradeType.Building && upgrade.CurrentLevel > -1)
                    buildings.Add((UpgradeTile)upgrade);
            return buildings;
        }

        public void RefundAll()
        {
            OnRefundAll?.Invoke();
        }

        public void UnlockAll()
        {
            OnUnlockAll?.Invoke();
        }

        public UpgradeBoardData SerializeSelf()
        {
            List<UpgradeData> upgradeDatas = new();
            foreach (Upgrade upgrade in Upgrades)
                upgradeDatas.Add(upgrade.SerializeSelf());

            UpgradeBoardData data = new()
            {
                UpgradeDatas = upgradeDatas
            };

            return data;
        }

        public void LoadFromData(UpgradeBoardData data)
        {
            Upgrades = new();
            foreach (Upgrade original in _upgradeOriginals)
            {
                UpgradeData upgradeData = data.UpgradeDatas.Find(u => u.Name == original.name);
                Upgrade instance = Instantiate(original);
                instance.name = original.name;
                if (upgradeData.Name != null) instance.LoadFromData(upgradeData);
                Upgrades.Add(instance);
            }
        }
    }

    [Serializable]
    public struct UpgradeBoardData
    {
        public List<UpgradeData> UpgradeDatas;
    }
}