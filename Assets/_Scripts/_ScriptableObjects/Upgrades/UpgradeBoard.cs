using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Board")]
public class UpgradeBoard : BaseScriptableObject
{
    [SerializeField] List<Upgrade> _upgradeOriginals = new();
    public List<Upgrade> Upgrades = new();

    Dictionary<string, Upgrade> _upgradeDictionary = new();

    public event Action OnRefundAll;
    public event Action OnUnlockAll;
    public void Initialize()
    {
        _upgradeDictionary = new();
        foreach (Upgrade upgrade in Upgrades)
            _upgradeDictionary.Add(upgrade.name, upgrade);
    }

    public Upgrade GetUpgradeByName(string name)
    {
        if (_upgradeDictionary.Count == 0) Initialize();

        if (_upgradeDictionary.ContainsKey(name))
            return _upgradeDictionary[name];

        return null;
    }

    public List<Upgrade> GetUpgradesByType(UpgradeType type)
    {
        List<Upgrade> upgrades = new();
        foreach (Upgrade upgrade in Upgrades)
            if (upgrade.Type == type)
                upgrades.Add(upgrade);
        return upgrades;
    }

    public void RefundAll() { OnRefundAll?.Invoke(); }
    public void UnlockAll() { OnUnlockAll?.Invoke(); }

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