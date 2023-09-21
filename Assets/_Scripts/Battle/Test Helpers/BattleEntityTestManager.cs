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
    Foldout _otherFoldout;

    public List<string> _entityTestLog = new();

    [SerializeField] List<Creature> _allCreatures = new();
    [SerializeField] Transform _teamASpawnPoint;
    [SerializeField] Transform _teamBSpawnPoint;

    [SerializeField] GameObject _chestPrefab;
    [SerializeField] ExperienceOrb[] _expOrbs;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _buttonContainer = _root.Q<VisualElement>("buttonContainer");


        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation("asd", _gameManager.EntityDatabase.GetRandomPortraitFemale(),
                 _gameManager.EntityDatabase.GetRandomElement());
        _gameManager.PlayerHero = newChar;

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandom(1);
        _gameManager.CurrentBattle = battle;

        StartCoroutine(LateInitialize(newChar));
    }

    IEnumerator LateInitialize(Hero h)
    {
        yield return new WaitForSeconds(0.5f);
        _battleManager.Initialize(h);
        _battleManager.GetComponent<BattleGrabManager>().Initialize();

        AddButtons();
    }

    void AddButtons()
    {
        AddHeroButtons();
        AddCreatureButtons();
        AddMinionButtons();

        AddSpawnChestButton();
        AddExpOrbButton();

        AddClearButton();
    }

    void AddCreatureButtons()
    {
        Foldout creatureFoldout = new()
        {
            text = "Creatures",
            value = false
        };
        _buttonContainer.Add(creatureFoldout);

        List<string> choices = new()
        {
            "Random",
            "-----"
        };
        choices.AddRange(_allCreatures.ConvertAll(x => x.name));
        var dropDownLeft = new DropdownField("Team 0", choices, 0);
        var dropDownRight = new DropdownField("Team 1", choices, 0);
        creatureFoldout.Add(dropDownLeft);
        creatureFoldout.Add(dropDownRight);

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
        creatureFoldout.Add(b);

        Button levelUpButton = new() { text = "Level Up" };
        levelUpButton.clickable.clicked += () =>
        {
            foreach (BattleEntity e in _battleManager.PlayerCreatures)
            {
                BattleCreature bc = (BattleCreature)e;
                bc.Creature.LevelUp();
            }
        };
        creatureFoldout.Add(levelUpButton);
    }

    void AddCreature(Creature c, int team)
    {
        c.InitializeBattle(team);
        BattleEntity be = SpawnEntity(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void AddRandomCreature(int team)
    {
        Creature c = Instantiate(_allCreatures[Random.Range(0, _allCreatures.Count)]);
        c.InitializeBattle(team);
        BattleEntity be = SpawnEntity(c, team == 0 ? _teamASpawnPoint.position : _teamBSpawnPoint.position);
        if (team == 0)
            _battleManager.AddPlayerArmyEntity(be);
        if (team == 1)
            _battleManager.AddOpponentArmyEntity(be);
    }

    void AddMinionButtons()
    {
        Foldout minionFoldout = new()
        {
            text = "Minions",
            value = false
        };
        _buttonContainer.Add(minionFoldout);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        TextField input = new() { value = "1" };
        Button bMinions = new() { text = "Spawn Minions" };
        bMinions.clickable.clicked += () =>
        {
            if (!int.TryParse(input.value, out int count)) return;

            for (int i = 0; i < count; i++)
            {
                Minion m = Instantiate(_gameManager.EntityDatabase.GetRandomMinion());

                m.InitializeBattle(1);
                BattleEntity be = SpawnEntity(m, _teamBSpawnPoint.position);
                _battleManager.AddOpponentArmyEntity(be);
            }
        };
        container.Add(input);
        container.Add(bMinions);
        minionFoldout.Add(container);
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

    void AddSpawnChestButton()
    {
        _otherFoldout = new()
        {
            text = "Other",
            value = false
        };
        _buttonContainer.Add(_otherFoldout);

        Button spawnChestButton = new() { text = "Spawn Chest" };
        spawnChestButton.clickable.clicked += () =>
        {
            GameObject instance = Instantiate(_chestPrefab, _teamBSpawnPoint.position, Quaternion.identity);
        };
        _otherFoldout.Add(spawnChestButton);
    }

    void AddExpOrbButton()
    {
        Button spawnExpOrbButton = new() { text = "Spawn Exp Orb" };
        spawnExpOrbButton.clickable.clicked += () =>
        {
            ExperienceOrb expOrb = Instantiate(_expOrbs[Random.Range(0, _expOrbs.Length)]);
            BattleExperienceOrb instance = Instantiate(expOrb.Prefab, _teamBSpawnPoint.position,
                                    Quaternion.identity).GetComponent<BattleExperienceOrb>();
            instance.Initialize(expOrb);
        };
        _otherFoldout.Add(spawnExpOrbButton);
    }

    void AddHeroButtons()
    {
        Foldout heroFoldout = new()
        {
            text = "Hero",
            value = false
        };
        _buttonContainer.Add(heroFoldout);

        Hero hero = _gameManager.PlayerHero;

        Button levelUpButton = new() { text = "Level up" };
        levelUpButton.clickable.clicked += () =>
        {
            hero.AddExp(hero.GetExpForNextLevel() - hero.Experience.Value);
        };
        heroFoldout.Add(levelUpButton);

        Button increaseGatherStrength = new() { text = "Increase Pull" };
        increaseGatherStrength.clickable.clicked += () =>
        {
            hero.Pull.SetBaseValue(hero.Pull.GetValue() + 1);
        };
        heroFoldout.Add(increaseGatherStrength);

        Button decreaseGatherStrength = new() { text = "Decrease Pull" };
        decreaseGatherStrength.clickable.clicked += () =>
        {
            hero.Pull.SetBaseValue(hero.Pull.GetValue() - 1);
        };
        heroFoldout.Add(decreaseGatherStrength);

        List<string> abilityChoices = new();
        abilityChoices.AddRange(_gameManager.EntityDatabase.GetAllAbilities().ConvertAll(x => x.name));
        var abilityDropdown = new DropdownField("Ability", abilityChoices, 0);
        Button b = new() { text = "Add Ability" };
        b.clickable.clicked += () =>
        {
            Ability a = _gameManager.EntityDatabase.GetAllAbilities().Find(x => x.name == abilityDropdown.value);
            if (a != null)
            {
                hero.AddAbility(a);
            }
        };

        heroFoldout.Add(abilityDropdown);
        heroFoldout.Add(b);

        Button abilityLevelUp = new() { text = "Level up abilities" };
        abilityLevelUp.clickable.clicked += () =>
        {
            foreach (Ability a in hero.Abilities)
            {
                a.LevelUp();
            }
        };
        heroFoldout.Add(abilityLevelUp);


    }
}

#endif
