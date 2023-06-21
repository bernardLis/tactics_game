using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ArmyGroup")]
public class ArmyGroup : BaseScriptableObject
{
    // I am going to remove army group now...
    public string Name;
    public Creature Creature;
    public int NumberOfUnits;
    public int ListPosition; // used for save/load of castle and hero army

    [HideInInspector] public int OldKillCount;
    [HideInInspector] public int TotalKillCount;

    [HideInInspector] public int OldDamageDealt;
    [HideInInspector] public int TotalDamageDealt;

    [HideInInspector] public int OldDamageTaken;
    [HideInInspector] public int TotalDamageTaken;

    [HideInInspector] public Creature PreviousCreature;

    public event Action<ArmyGroup> OnEvolved;

    public event Action<int, int> OnCountChanged;
    public void ChangeCount(int change)
    {
        NumberOfUnits += change;
        OnCountChanged?.Invoke(ListPosition, NumberOfUnits);
    }

    public void JoinArmy(ArmyGroup armyGroup)
    {
        if (armyGroup.Creature != Creature) return;

        ChangeCount(armyGroup.NumberOfUnits);
        TotalDamageDealt += armyGroup.TotalDamageDealt;
        TotalDamageTaken += armyGroup.TotalDamageTaken;
        TotalKillCount += armyGroup.TotalKillCount;
    }

    public void InitializeBattle()
    {
        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
        OldDamageTaken = TotalDamageTaken;
    }

    public void AddKill(int ignored)
    {
        TotalKillCount++;
    }

    public void AddDmgDealt(int dmg)
    {
        TotalDamageDealt += dmg;
    }

    public void AddDmgTaken(int dmg)
    {
        TotalDamageTaken += dmg;
    }

    public int NumberOfKillsToEvolve()
    {
        return Creature.KillsToUpgrade * NumberOfUnits;
    }

    public bool ShouldEvolve()
    {
        if (Creature.UpgradedCreature == null) return false;

        return TotalKillCount >= NumberOfKillsToEvolve();
    }

    public void Evolve()
    {
        PreviousCreature = Creature;
        Creature = Creature.UpgradedCreature;
        OnEvolved?.Invoke(this);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.Name = Name;
        data.CreatureId = Creature.Id;
        data.NumberOfUnits = NumberOfUnits;
        data.ListPosition = ListPosition;

        data.KillCount = TotalKillCount;
        data.DamageDealt = TotalDamageDealt;
        data.DamageTaken = TotalDamageTaken;

        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        Creature = GameManager.Instance.HeroDatabase.GetCreatureById(data.CreatureId);
        Name = data.Name;
        NumberOfUnits = data.NumberOfUnits;
        ListPosition = data.ListPosition;

        TotalKillCount = data.KillCount;
        TotalDamageDealt = data.DamageDealt;
        TotalDamageTaken = data.DamageTaken;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string Name;
    public string CreatureId;
    public int NumberOfUnits;
    public int ListPosition;

    public int KillCount;
    public int DamageDealt;
    public int DamageTaken;
}

