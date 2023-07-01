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

    public List<string> _entityTestLog = new();

    [Space(10)]
    [SerializeField] TestType _testType;

    [Space(10)]
    [Header("Specific Armies")]
    [SerializeField] List<Creature> _teamACreatures = new();
    [SerializeField] List<Creature> _teamBCreatures = new();

    [Space(10)]
    [Header("One Army vs All")]
    [SerializeField] List<Creature> _oneCreature = new();

    [Space(10)]
    [Header("Battle Setup")]
    [SerializeField] List<Creature> _allCreatures = new();
    [SerializeField] Transform _teamASpawnPoint;
    [SerializeField] Transform _teamBSpawnPoint;

    List<BattleEntity> _teamA = new();
    List<BattleEntity> _teamB = new();

    int _currentAllCreaturesIndex;
    int _currentFullOneVOneIndex;

    List<Creature> _defeatedCreatures = new();
    List<Creature> _lostToCreatures = new();

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _currentAllCreaturesIndex = -1;

        ResolveTestType();


        /*
        else
        {
            _allCreatures.AddRange(_battleManager.CreatureDatabase.GetAllCreatures());
        }

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
        */
    }

    void ResolveTestType()
    {
        _battleManager.ClearAllEntities();

        if (_testType == TestType.SpecificTeams)
        {
            _entityTestLog.Add($"{Time.time}: Running Specific Teams Test ");
            ResolveTeams(_teamACreatures, _teamBCreatures);
        }
        else if (_testType == TestType.OneCreatureVsAll)
        {
            _currentAllCreaturesIndex++;
            if (_currentAllCreaturesIndex >= _allCreatures.Count)
            {
                _entityTestLog.Add($"{Time.time}: Finished One Creature Vs All Test.");
                return;
            }

            ResolveOneCreatureVsAll(_oneCreature);
        }
        else if (_testType == TestType.FullOneVOne)
        {
            _entityTestLog.Add($"{Time.time}: Running Full One Vs One Test. Index: {_currentAllCreaturesIndex}");

            _currentAllCreaturesIndex++;
            if (_currentAllCreaturesIndex >= _allCreatures.Count)
            {
                _currentFullOneVOneIndex++;
                _currentAllCreaturesIndex = 0;
            }

            if (_currentFullOneVOneIndex >= _allCreatures.Count)
            {
                _entityTestLog.Add($"{Time.time}: Finished Full One Vs One Test.");
                return;
            }

            List<Creature> teamA = new();
            teamA.Add(_allCreatures[_currentFullOneVOneIndex]);

            ResolveOneCreatureVsAll(teamA);
        }
    }

    void ResolveOneCreatureVsAll(List<Creature> oneArmy)
    {
        _entityTestLog.Add($"{Time.time}: Running One Army Vs All Test. Index: {_currentAllCreaturesIndex}");
        List<Creature> teamB = new();
        teamB.Add(_allCreatures[_currentAllCreaturesIndex]);
        ResolveTeams(oneArmy, teamB);

    }

    void ResolveTeams(List<Creature> teamACreatures, List<Creature> teamBCreatures)
    {
        _teamA = new();
        _teamB = new();

        foreach (Creature c in teamACreatures)
        {
            BattleEntity be = SpawnCreature(c, _teamASpawnPoint.position);
            _teamA.Add(be);
            be.OnDeath += OnTeamADeath;
        }
        foreach (Creature c in teamBCreatures)
        {
            BattleEntity be = SpawnCreature(c, _teamBSpawnPoint.position);
            _teamB.Add(be);
            be.OnDeath += OnTeamBDeath;
        }

        string teamANames = "";
        string teamBNames = "";

        foreach (Creature c in teamACreatures)
            teamANames += $"{c.Name}, ";
        foreach (Creature c in teamBCreatures)
            teamBNames += $"{c.Name}, ";

        _entityTestLog.Add($"Team A: {teamANames} vs Team B: {teamBNames}");

        _battleManager.Initialize(null, _teamA, _teamB);
    }

    BattleEntity SpawnCreature(Creature c, Vector3 spawnPos)
    {
        Vector3 pos = spawnPos + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        GameObject instance = Instantiate(c.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeCreature(c);
        return be;
    }

    void OnTeamADeath(BattleEntity self, BattleEntity killer, Ability killerAbility)
    {
        _teamA.Remove(self);
        if (_teamA.Count == 0)
        {
            string creaturesLeft = "";
            foreach (BattleEntity be in _teamB)
                creaturesLeft += $"{be.Creature.Name}, ";

            _entityTestLog.Add($"{Time.time}: Team B won! {creaturesLeft} left.");
            ResolveTestType();
        }
    }

    void OnTeamBDeath(BattleEntity self, BattleEntity killer, Ability killerAbility)
    {
        _teamB.Remove(self);
        if (_teamB.Count == 0)
        {
            string creaturesLeft = "";
            foreach (BattleEntity be in _teamA)
                creaturesLeft += $"{be.Creature.Name}, ";

            _entityTestLog.Add($"{Time.time}: Team A won! {creaturesLeft} left.");
            ResolveTestType();
        }
    }




    /*
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

    */
}


public enum TestType
{
    SpecificTeams,
    OneCreatureVsAll,
    FullOneVOne,
}

#endif
