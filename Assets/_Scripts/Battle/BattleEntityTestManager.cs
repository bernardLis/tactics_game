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

    [SerializeField] GameObject _obstacle;

    [SerializeField] List<ArmyGroup> AllGroups = new();
    int _currentGroupIndex = 0;

    [Header("Specific Armies")]
    [SerializeField] bool _testSpecificTeams;
    [SerializeField] List<ArmyGroup> TeamAArmies = new();
    [SerializeField] List<ArmyGroup> TeamBArmies = new();

    [Header("One Army vs All")]
    [SerializeField] bool _oneArmyVsAll;
    [SerializeField] List<ArmyGroup> _oneArmy = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
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
        if (_currentGroupIndex == AllGroups.Count)
        {
            Debug.Log("Test finished");
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
            _obstacle.SetActive(false);
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
