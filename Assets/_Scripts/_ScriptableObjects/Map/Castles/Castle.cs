using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Castle")]
public class Castle : BaseScriptableObject
{
    public static int MaxCastleArmySlots = 5;

    public string TemplateCastleId;
    public Sprite Sprite;
    public Vector2 MapPosition;
    public bool IsOwnedByPlayer;
    public bool HasBuiltToday;
    public List<Building> Buildings = new();
    public List<ArmyGroup> Army = new();

    GameManager _gameManager;

    public event Action OnBuilt;
    public void Initialize()
    {
        foreach (Building b in Buildings)
        {
            b.Initialize();
            b.OnBuilt += BuildingBuilt;
        }

        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void BuildingBuilt()
    {
        HasBuiltToday = true;
        OnBuilt?.Invoke();
    }

    void OnDayPassed(int day) { HasBuiltToday = false; }

    public void AddArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Castle {name} adds army {armyGroup.ArmyEntity} count {armyGroup.EntityCount}");

        Army.Add(armyGroup);
    }

    public void RemoveArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Castle {name} removes {armyGroup.ArmyEntity} count {armyGroup.EntityCount}");

        Army.Remove(armyGroup);
    }

    public void Create(string templateCastleId, Vector2 mapPosition)
    {
        Id = System.Guid.NewGuid().ToString();
        Debug.Log($"create castle is called with {templateCastleId} and {mapPosition}");
        _gameManager = GameManager.Instance;
        TemplateCastleId = templateCastleId;
        Castle templateCastle = _gameManager.GameDatabase.GetCastleById(templateCastleId);
        Sprite = templateCastle.Sprite;
        MapPosition = mapPosition;
        foreach (Building b in templateCastle.Buildings)
        {
            Building instance = Instantiate(_gameManager.GameDatabase.GetBuildingById(b.Id));
            instance.Initialize();
            Buildings.Add(instance);
        }
        Army = new();
    }

    public CastleData SerializeSelf()
    {
        CastleData data = new();

        data.Id = Id;
        data.TemplateCastleId = TemplateCastleId;
        data.MapPosition = MapPosition;
        data.IsOwnedByPlayer = IsOwnedByPlayer;
        data.HasBuiltToday = HasBuiltToday;

        data.BuildingDatas = new();
        foreach (Building b in Buildings)
        {
            BuildingData bd = b.SerializeSelf();
            data.BuildingDatas.Add(bd);
        }

        data.AvailableArmyDatas = new();
        foreach (ArmyGroup g in Army)
        {
            ArmyGroupData gd = g.SerializeSelf();
            data.AvailableArmyDatas.Add(gd);
        }

        return data;
    }

    public void LoadFromData(CastleData data)
    {
        _gameManager = GameManager.Instance;

        Id = data.Id;
        name = "Castle";

        TemplateCastleId = data.TemplateCastleId;
        Castle templateCastle = _gameManager.GameDatabase.GetCastleById(TemplateCastleId);
        Sprite = templateCastle.Sprite;

        MapPosition = data.MapPosition;
        IsOwnedByPlayer = data.IsOwnedByPlayer;
        HasBuiltToday = data.HasBuiltToday;

        foreach (BuildingData d in data.BuildingDatas)
        {
            Building b = Instantiate(_gameManager.GameDatabase.GetBuildingById(d.Id));
            b.LoadFromData(d);
            Buildings.Add(b);
        }
        foreach (ArmyGroupData d in data.AvailableArmyDatas)
        {
            ArmyGroup g = ScriptableObject.CreateInstance<ArmyGroup>();
            g.LoadFromData(d);
            Army.Add(g);
        }
    }
}

[System.Serializable]
public struct CastleData
{
    public string Id;
    public string TemplateCastleId;
    public Vector2 MapPosition;
    public bool IsOwnedByPlayer;
    public bool HasBuiltToday;
    public List<BuildingData> BuildingDatas;
    public List<ArmyGroupData> AvailableArmyDatas;
}


