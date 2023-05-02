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

    [SerializeField]
    List<ArmyGroup> AllGroups = new();
    int _currentGroupIndex = 0;

    [Space(10)]
    [SerializeField] bool _testSpecificTeams;
    [SerializeField] List<ArmyGroup> TeamAArmies = new();
    [SerializeField] List<ArmyGroup> TeamBArmies = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;
        if (_testSpecificTeams)
        {
            _battleManager.Initialize(null, null, TeamAArmies, TeamBArmies);
            return;
        }

        RunTest();
    }

    void RunTest()
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
        RunTest();
    }


}
