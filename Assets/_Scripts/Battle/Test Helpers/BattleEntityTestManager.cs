using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Random = UnityEngine.Random;
#if (UNITY_EDITOR) 

public class BattleEntityTestManager : MonoBehaviour
{
    BattleManager _battleManager;

    [SerializeField] List<Creature> _allCreatures = new();
    int _currentGroupIndex = 0;

    [Header("Specific Armies")]
    [SerializeField] bool _testSpecificTeams;
    [SerializeField] List<Creature> TeamACreatures = new();
    [SerializeField] List<Creature> TeamBCreatures = new();

    [Header("One Army vs All")]
    [SerializeField] bool _oneCreatureVsAll;
    [SerializeField] List<Creature> _oneCreature = new();

    [SerializeField] bool _fullOneVOnes;
    int _currentOneVOneIndex = 0;


    List<Creature> _defeatedCreatures = new();
    List<Creature> _lostToCreatures = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.IsEndingBattleBlocked = true;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        if (_testSpecificTeams)
        {
            // HERE: creature spawning _battleManager.Initialize(null, null, TeamACreatures, TeamBCreatures);
            return;
        }
        if (_oneCreatureVsAll)
        {
            RunOneArmyVsAll();
            return;
        }
        if (_fullOneVOnes)
        {
            _oneCreatureVsAll = true;
            _oneCreature = null;
            _oneCreature = new();
            _oneCreature.Add(_allCreatures[0]);
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
                _defeatedCreatures.Add(_allCreatures[_currentGroupIndex - 1]);
            else
                _lostToCreatures.Add(_allCreatures[_currentGroupIndex - 1]);
            _battleManager.LoadedBattle.Won = false;
        }

        if (_currentGroupIndex == _allCreatures.Count)
        {
            Debug.Log("Test finished");

            string s = $"{_allCreatures[0].name} defeated: ";
            foreach (Creature entity in _defeatedCreatures)
                s += $"{entity.name}, ";
            s += "\n";
            s += $"Lost to: ";
            foreach (Creature entity in _lostToCreatures)
                s += $"{entity.name}, ";
            Debug.Log($"{s}");
            EndOfOneArmyVsAll();
            return;
        }

        List<Creature> teamB = new();
        teamB.Add(_allCreatures[_currentGroupIndex]);
        // HERE: creature spawning   _battleManager.Initialize(null, null, _oneCreature, teamB);
        _currentGroupIndex++;
    }

    void EndOfOneArmyVsAll()
    {
        if (!_fullOneVOnes) return;
        _currentOneVOneIndex++;
        if (_currentOneVOneIndex == _allCreatures.Count)
        {
            Debug.Log("Full 1v1 finished");
            return;
        }
        _defeatedCreatures.Clear();
        _lostToCreatures.Clear();
        _currentGroupIndex = 0;
        _oneCreature.Clear();
        _oneCreature.Add(_allCreatures[_currentOneVOneIndex]);
        RunOneArmyVsAll();
    }

    void RunAllGroups()
    {
        Debug.Log($"Running test index: {_currentGroupIndex}");
        if (_currentGroupIndex == _allCreatures.Count)
        {
            Debug.Log("Test finished");
            return;
        }

        List<Creature> teamA = new();
        teamA.Add(_allCreatures[_currentGroupIndex]);
        List<Creature> teamB = new();
        teamB.Add(_allCreatures[_currentGroupIndex]);
        // HERE: creature spawning    _battleManager.Initialize(null, null, teamA, teamB);
        _currentGroupIndex++;
    }

    void OnBattleFinalized()
    {
        if (_testSpecificTeams)
        {
            // HERE: creature spawning     _battleManager.Initialize(null, null, TeamACreatures, TeamBCreatures);
            return;
        }

        if (_oneCreatureVsAll)
        {
            RunOneArmyVsAll();
            return;
        }

        RunAllGroups();
    }


}
#endif
