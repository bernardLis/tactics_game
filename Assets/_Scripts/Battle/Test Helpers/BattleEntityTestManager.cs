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

    [SerializeField] bool _fullOneVOnes;
    int _currentOneVOneIndex = 0;


    List<Creature> _defeatedEntities = new();
    List<Creature> _lostToEntities = new();

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
        if (_fullOneVOnes)
        {
            _oneArmyVsAll = true;
            _oneArmy.Clear();
            _oneArmy.Add(AllGroups[0]);
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
                _defeatedEntities.Add(AllGroups[_currentGroupIndex - 1].Creature);
            else
                _lostToEntities.Add(AllGroups[_currentGroupIndex - 1].Creature);
            _battleManager.LoadedBattle.Won = false;
        }

        if (_currentGroupIndex == AllGroups.Count)
        {
            Debug.Log("Test finished");

            string s = $"{_oneArmy[0].Creature.name} defeated: ";
            foreach (Creature entity in _defeatedEntities)
                s += $"{entity.name}, ";
            s += "\n";
            s += $"Lost to: ";
            foreach (Creature entity in _lostToEntities)
                s += $"{entity.name}, ";
            Debug.Log($"{s}");
            EndOfOneArmyVsAll();
            return;
        }

        List<ArmyGroup> teamB = new();
        teamB.Add(AllGroups[_currentGroupIndex]);
        _battleManager.Initialize(null, null, _oneArmy, teamB);
        _currentGroupIndex++;
    }

    void EndOfOneArmyVsAll()
    {
        if (!_fullOneVOnes) return;
        _currentOneVOneIndex++;
        if (_currentOneVOneIndex == AllGroups.Count)
        {
            Debug.Log("Full 1v1 finished");
            return;
        }
        _defeatedEntities.Clear();
        _lostToEntities.Clear();
        _currentGroupIndex = 0;
        _oneArmy.Clear();
        _oneArmy.Add(AllGroups[_currentOneVOneIndex]);
        RunOneArmyVsAll();
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
