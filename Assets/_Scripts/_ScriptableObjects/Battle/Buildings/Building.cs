using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building")]
public class Building : BaseScriptableObject
{
    public Sprite Icon;
    public bool IsSecured;
    public GlobalUpgrade BuildingUpgrade;

    public int SecondsToCorrupt;

    public event Action OnSecured;
    public event Action OnCorrupted;

    public GameObject BuildingPrefab;
    public GameObject TileIndicatorPrefab;


    public virtual void Initialize()
    {
        BuildingUpgrade = GameManager.Instance.GlobalUpgradeBoard
                        .GetBuildingUpgradeByName(name);
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
