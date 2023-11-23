using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;

public class CommandLineManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    VisualElement _commandLineContainer;
    TextField _commandTextField;
    Button _submitCommandButton;

    ScrollView _logContainer;
    static string myLog = "";

    bool _isOpen;

    Label _fpsLabel;
    float _deltaTime;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _playerInput = GetComponent<PlayerInput>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _commandLineContainer = root.Q<VisualElement>("commandLineContainer");
        _commandTextField = root.Q<TextField>("commandLineTextField");
        _submitCommandButton = root.Q<Button>("commandLineButton");
        _submitCommandButton.clickable.clicked += SubmitCommand;

        _logContainer = root.Q<ScrollView>("logContainer");

        _fpsLabel = root.Q<Label>("fpsLabel");

        AddButtons();
    }

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsLabel.text = $"{Mathf.Ceil(fps)}";
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();

        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;
        UnsubscribeInputActions();

        Application.logMessageReceived -= Log;
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["ToggleCommandLine"].performed += ToggleCommandLine;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["ToggleCommandLine"].performed -= ToggleCommandLine;
    }

    void ToggleCommandLine(InputAction.CallbackContext ctx)
    {
        if (_isOpen)
        {
            _commandLineContainer.style.display = DisplayStyle.None;
            _buttonContainer.style.display = DisplayStyle.None;
        }
        else
        {
            _commandLineContainer.style.display = DisplayStyle.Flex;
            _buttonContainer.style.display = DisplayStyle.Flex;
        }

        _isOpen = !_isOpen;
    }

    /* BUTTONS */
    VisualElement _buttonContainer;
    Foldout _otherFoldout;
    [SerializeField] List<Creature> _allCreatures = new();
    [SerializeField] GameObject _chestPrefab;
    [SerializeField] ExperienceOrb[] _expOrbs;

    void AddButtons()
    {
        _buttonContainer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("commandButtonsContainer");

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
            foreach (BattleEntity e in BattleManager.Instance.PlayerCreatures)
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
        BattleEntity be = SpawnEntity(c, team, Vector3.zero);
        if (team == 0)
            BattleManager.Instance.AddPlayerArmyEntity(be);
        if (team == 1)
            BattleManager.Instance.AddOpponentArmyEntity(be);
    }

    void AddRandomCreature(int team)
    {
        Creature c = Instantiate(_allCreatures[Random.Range(0, _allCreatures.Count)]);
        c.InitializeBattle(team);
        BattleEntity be = SpawnEntity(c, team, Vector3.zero);
        if (team == 0)
            BattleManager.Instance.AddPlayerArmyEntity(be);
        if (team == 1)
            BattleManager.Instance.AddOpponentArmyEntity(be);
    }

    void AddMinionButtons()
    {
        BattleManager battleManager = BattleManager.Instance;

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
                BattleEntity be = SpawnEntity(m, 1, Vector3.zero);
                battleManager.AddOpponentArmyEntity(be);
            }
        };
        container.Add(input);
        container.Add(bMinions);
        minionFoldout.Add(container);
    }


    BattleEntity SpawnEntity(Entity entity, int team, Vector3 spawnPos)
    {
        Vector3 pos = spawnPos + new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
        GameObject instance = Instantiate(entity.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity, team);
        return be;
    }

    void AddClearButton()
    {
        BattleManager battleManager = BattleManager.Instance;

        Button clearButton = new() { text = "Clear" };
        clearButton.clickable.clicked += () =>
        {
            List<BattleEntity> collection = new(battleManager.PlayerCreatures);
            collection.AddRange(battleManager.OpponentEntities);
            foreach (BattleEntity e in collection)
                e.TriggerDieCoroutine();
            Invoke(nameof(Clear), 1f);
        };
        _buttonContainer.Add(clearButton);
    }

    void Clear()
    {
        BattleManager.Instance.ClearAllEntities();
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
            GameObject instance = Instantiate(_chestPrefab, Vector3.zero, Quaternion.identity);
        };
        _otherFoldout.Add(spawnChestButton);
    }

    void AddExpOrbButton()
    {
        Button spawnExpOrbButton = new() { text = "Spawn Exp Orb" };
        spawnExpOrbButton.clickable.clicked += () =>
        {
            ExperienceOrb expOrb = Instantiate(_expOrbs[Random.Range(0, _expOrbs.Length)]);
            BattleExperienceOrb instance = Instantiate(expOrb.Prefab, Vector3.zero,
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


    /* COMMANDS */
    void SubmitCommand()
    {
        if (_commandTextField.text.ToLower() == "quit")
            Application.Quit();
        if (_commandTextField.text.ToLower() == "clearsave")
            _gameManager.ClearSaveData();

        if (_commandTextField.text.ToLower() == "warren")
            _gameManager.ChangeGoldValue(10000);
        if (_commandTextField.text.ToLower() == "takegold")
            _gameManager.ChangeGoldValue(-5000);
        if (_commandTextField.text.ToLower() == "levelup")
            _gameManager.PlayerHero.LevelUp();
        if (_commandTextField.text.ToLower() == "killbill")
            KillAllPlayerCreatures();
        if (_commandTextField.text.ToLower() == "tween")
            DoTweenSeeAllTweens();

    }

    void KillAllPlayerCreatures()
    {
        BattleManager battleManager = BattleManager.Instance;
        if (battleManager == null) return;
        List<BattleEntity> creatures = new(battleManager.PlayerCreatures);
        foreach (BattleCreature creature in creatures)
            creature.TriggerDeath();
    }

    void DoTweenSeeAllTweens()
    {
        foreach (Tween tween in DOTween.PlayingTweens())
        {
            Debug.Log($"{tween} is tweening | tween target: {tween.target}");
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (_logContainer == null) return;
        _logContainer.Add(new Label(logString));

        myLog = logString + "\n" + myLog;
        if (myLog.Length > 5000)
            myLog = myLog.Substring(0, 4000);

        FileManager.WriteToFile("log", myLog);
    }

}
