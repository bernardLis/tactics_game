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

    VisualElement _root;
    VisualElement _buttonContainer;

    public List<string> _entityTestLog = new();

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
        _root = GetComponent<UIDocument>().rootVisualElement;
        _buttonContainer = _root.Q<VisualElement>("buttonContainer");

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;
        _currentAllCreaturesIndex = -1;

        GameManager gameManager = GameManager.Instance;

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation("asd", gameManager.HeroDatabase.GetRandomPortraitFemale(),
                 gameManager.HeroDatabase.GetRandomElement());
        gameManager.PlayerHero = newChar;

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandom(1);
        gameManager.SelectedBattle = battle;

        StartCoroutine(LateInitialize(newChar));
    }

    IEnumerator LateInitialize(Hero h)
    {
        yield return new WaitForSeconds(0.5f);
        _battleManager.Initialize(h, new Vector3(-60f, 0, 60f));
        _battleManager.GetComponent<BattleGrabManager>().Initialize();
        _battleManager.GetComponent<BattleAbilityManager>().Initialize(h);

        AddButtons();
    }

    void AddButtons()
    {
        List<string> choices = new()
        {
            "Random",
            "-----"
        };
        choices.AddRange(_allCreatures.ConvertAll(x => x.Name));
        var dropDownLeft = new DropdownField("Left Creature", choices, 0);
        var dropDownRight = new DropdownField("Right Creature", choices, 0);
        _buttonContainer.Add(dropDownLeft);
        _buttonContainer.Add(dropDownRight);

        Button b = new() { text = "Spawn" };
        b.clickable.clicked += () =>
        {
            if (dropDownLeft.value == "Random")
                AddRandomCreature(0);
            if (dropDownRight.value == "Random")
                AddRandomCreature(1);

            // try to get creature from string name
            Creature cLeft = _allCreatures.Find(x => x.Name == dropDownLeft.value);
            if (cLeft != null)
            {
                Creature instance = Instantiate(cLeft);
                AddCreature(instance, 0);
            }
            Creature cRight = _allCreatures.Find(x => x.Name == dropDownRight.value);
            if (cRight != null)
            {
                Creature instance = Instantiate(cRight);
                AddCreature(instance, 1);
            }
        };
        _buttonContainer.Add(b);


        /*
                Button b = new() { text = "Random Pair" };
                b.clickable.clicked += () =>
                {
                    AddRandomCreature(0);
                    AddRandomCreature(1);
                };
                _buttonContainer.Add(b);

                Button bLeft = new() { text = "Random Left" };
                bLeft.clickable.clicked += () =>
                {
                    AddRandomCreature(0);
                };
                _buttonContainer.Add(bLeft);

                Button bRight = new() { text = "Random Right" };
                bRight.clickable.clicked += () =>
                {
                    AddRandomCreature(1);
                };
                _buttonContainer.Add(bRight);
                */
    }

    void AddCreature(Creature c, int team)
    {
        c.InitializeBattle(null);
        BattleEntity be = SpawnCreature(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void AddRandomCreature(int team)
    {
        Creature c = _allCreatures[Random.Range(0, _allCreatures.Count)];
        c.InitializeBattle(null);
        BattleEntity be = SpawnCreature(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void ResolveTestType()
    {
        _battleManager.ClearAllEntities();

        /*
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
                */
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

        _battleManager.Initialize(null);
        _battleManager.AddPlayerArmyEntities(_teamA);
        _battleManager.AddOpponentArmyEntities(_teamB);
    }

    BattleEntity SpawnCreature(Creature c, Vector3 spawnPos)
    {
        Vector3 pos = spawnPos + new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
        GameObject instance = Instantiate(c.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(c);
        return be;
    }

    void OnTeamADeath(BattleEntity self, GameObject killer)
    {
        _teamA.Remove(self);
        if (_teamA.Count == 0)
        {
            string creaturesLeft = "";
            foreach (BattleEntity be in _teamB)
                creaturesLeft += $"{be.Entity.Name}, ";

            _entityTestLog.Add($"{Time.time}: Team B won! {creaturesLeft} left.");
            ResolveTestType();
        }
    }

    void OnTeamBDeath(BattleEntity self, GameObject killer)
    {
        _teamB.Remove(self);
        if (_teamB.Count == 0)
        {
            string creaturesLeft = "";
            foreach (BattleEntity be in _teamA)
                creaturesLeft += $"{be.Entity.Name}, ";

            _entityTestLog.Add($"{Time.time}: Team A won! {creaturesLeft} left.");
            ResolveTestType();
        }
    }
}


public enum TestType
{
    SpecificTeams,
    OneCreatureVsAll,
    FullOneVOne,
}

#endif
