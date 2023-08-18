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
    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _root;
    VisualElement _buttonContainer;

    public List<string> _entityTestLog = new();

    [SerializeField] List<Creature> _allCreatures = new();
    [SerializeField] Transform _teamASpawnPoint;
    [SerializeField] Transform _teamBSpawnPoint;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _buttonContainer = _root.Q<VisualElement>("buttonContainer");

        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation("asd", _gameManager.HeroDatabase.GetRandomPortraitFemale(),
                 _gameManager.HeroDatabase.GetRandomElement());
        _gameManager.PlayerHero = newChar;

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandom(1);
        _gameManager.SelectedBattle = battle;

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
        AddCreatureButtons();
        AddMinionButtons();
        AddClearButton();
    }

    void AddCreatureButtons()
    {
        List<string> choices = new()
        {
            "Random",
            "-----"
        };
        choices.AddRange(_allCreatures.ConvertAll(x => x.name));
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
            Creature cLeft = _allCreatures.Find(x => x.name == dropDownLeft.value);
            if (cLeft != null)
            {
                Creature instance = Instantiate(cLeft);
                AddCreature(instance, 0);
            }
            Creature cRight = _allCreatures.Find(x => x.name == dropDownRight.value);
            if (cRight != null)
            {
                Creature instance = Instantiate(cRight);
                AddCreature(instance, 1);
            }
        };
        _buttonContainer.Add(b);

        Button levelUpButton = new() { text = "Level Up" };
        levelUpButton.clickable.clicked += () =>
        {
            foreach (BattleEntity e in _battleManager.PlayerCreatures)
            {
                BattleCreature bc = (BattleCreature)e;
                bc.Creature.LevelUp();
            }
        };
        _buttonContainer.Add(levelUpButton);

        _buttonContainer.Add(new Label("-----"));
    }

    void AddCreature(Creature c, int team)
    {
        c.InitializeBattle(team == 0 ? _gameManager.PlayerHero : null);
        BattleEntity be = SpawnEntity(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void AddRandomCreature(int team)
    {
        Creature c = Instantiate(_allCreatures[Random.Range(0, _allCreatures.Count)]);
        c.InitializeBattle(team == 0 ? _gameManager.PlayerHero : null);
        BattleEntity be = SpawnEntity(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void AddMinionButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        TextField input = new() { value = "1" };
        Button bMinions = new() { text = "Spawn Minions" };
        bMinions.clickable.clicked += () =>
        {
            int count = int.Parse(input.value);

            for (int i = 0; i < count; i++)
            {
                Minion m = Instantiate(_gameManager.HeroDatabase.GetRandomMinion());

                m.InitializeBattle(null);
                BattleEntity be = SpawnEntity(m, _teamBSpawnPoint.position);
                _battleManager.AddOpponentArmyEntity(be);
            }
        };
        container.Add(input);
        container.Add(bMinions);
        _buttonContainer.Add(container);
        _buttonContainer.Add(new Label("-----"));
    }


    BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
    {
        Vector3 pos = spawnPos + new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
        GameObject instance = Instantiate(entity.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity);
        return be;
    }

    void AddClearButton()
    {
        Button clearButton = new() { text = "Clear" };
        clearButton.clickable.clicked += () =>
        {
            List<BattleEntity> collection = new(_battleManager.PlayerCreatures);
            collection.AddRange(_battleManager.OpponentEntities);
            foreach (BattleEntity e in collection)
                e.TriggerDieCoroutine();
            Invoke(nameof(Clear), 1f);
        };
        _buttonContainer.Add(clearButton);

    }
    void Clear()
    {
        _battleManager.ClearAllEntities();
    }
}

#endif
