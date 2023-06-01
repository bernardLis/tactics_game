using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Random = UnityEngine.Random;

public class BattleEntityTestManager : MonoBehaviour
{
    BattleManager _battleManager;

    [SerializeField] List<ArmyGroup> AllGroups = new();
    int _currentGroupIndex = 0;

    [Header("Specific Armies")]
    [SerializeField] bool _testSpecificTeams;
    [SerializeField] List<ArmyGroup> TeamAArmies = new();
    [SerializeField] List<ArmyGroup> TeamBArmies = new();

    [Header("One Army vs All")]
    [SerializeField] bool _oneArmyVsAll;
    [SerializeField] List<ArmyGroup> _oneArmy = new();

    List<ArmyEntity> defeatedEntities = new();
    List<ArmyEntity> lostToEntities = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.IsEndingBattleBlocked = true;
        _battleManager.OnBattleFinalized += OnBattleFinalized;
        if (_testSpecificTeams)
        {
            _battleManager.Initialize(null, null, TeamAArmies, TeamBArmies);
            return;
        }
        if (_oneArmyVsAll)
        {
            RunOneArmyVsAll();
            return;
        }

        RunAllGroups();
    }

    void RunOneArmyVsAll()
    {
        Debug.Log($"Running test index: {_currentGroupIndex}");

        if (_currentGroupIndex > 0)
        {
            if (_battleManager.LoadedBattle.Won)
                defeatedEntities.Add(AllGroups[_currentGroupIndex - 1].ArmyEntity);
            else
                lostToEntities.Add(AllGroups[_currentGroupIndex - 1].ArmyEntity);
            _battleManager.LoadedBattle.Won = false;
        }

        if (_currentGroupIndex == AllGroups.Count)
        {
            Debug.Log("Test finished");

            string s = $"{_oneArmy[0].ArmyEntity.name} defeated: ";
            foreach (ArmyEntity entity in defeatedEntities)
                s += $"{entity.name}, ";
            s += "\n";
            s += $"Lost to: ";
            foreach (ArmyEntity entity in lostToEntities)
                s += $"{entity.name}, ";
            Debug.Log($"{s}");
            return;
        }


        List<ArmyGroup> teamB = new();
        teamB.Add(AllGroups[_currentGroupIndex]);
        _battleManager.Initialize(null, null, _oneArmy, teamB);
        _currentGroupIndex++;
    }

    void RunAllGroups()
    {
        Debug.Log($"Running test index: {_currentGroupIndex}");
        if (_currentGroupIndex == AllGroups.Count)
        {
            Debug.Log("Test finished");
            return;
        }

        List<ArmyGroup> teamA = new();
        teamA.Add(AllGroups[_currentGroupIndex]);
        List<ArmyGroup> teamB = new();
        teamB.Add(AllGroups[_currentGroupIndex]);
        _battleManager.Initialize(null, null, teamA, teamB);
        _currentGroupIndex++;
    }

    void OnBattleFinalized()
    {
        if (_testSpecificTeams)
        {
            _battleManager.Initialize(null, null, TeamAArmies, TeamBArmies);
            return;
        }

        if (_oneArmyVsAll)
        {
            RunOneArmyVsAll();
            return;
        }

        RunAllGroups();
    }


}
