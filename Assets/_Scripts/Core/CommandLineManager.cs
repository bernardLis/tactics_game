using System.Collections.Generic;
using DG.Tweening;
using Lis.Arena;
using Lis.Arena.Fight;
using Lis.Arena.Pickup;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Enemy;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class CommandLineManager : MonoBehaviour
    {
        static string _myLog = "";

        /* BUTTONS */
        VisualElement _buttonContainer;

        bool _buttonsAdded;

        VisualElement _commandLineContainer;
        TextField _commandTextField;
        float _deltaTime;

        Label _fpsLabel;
        GameManager _gameManager;

        bool _isOpen;

        ScrollView _logContainer;
        Foldout _otherFoldout;
        PlayerInput _playerInput;
        Button _submitCommandButton;
        UnitDatabase _unitDatabase;

        void Start()
        {
            _gameManager = GetComponent<GameManager>();
            _playerInput = GetComponent<PlayerInput>();

            _unitDatabase = _gameManager.UnitDatabase;

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            _commandLineContainer = root.Q<VisualElement>("commandLineContainer");
            _commandTextField = root.Q<TextField>("commandLineTextField");
            _submitCommandButton = root.Q<Button>("commandLineButton");
            _submitCommandButton.clickable.clicked += SubmitCommand;

            _logContainer = root.Q<ScrollView>("logContainer");

            _fpsLabel = root.Q<Label>("fpsLabel");

            TryAddingButtons(default, default);
            SceneManager.sceneLoaded += TryAddingButtons;
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

        void TryAddingButtons(Scene scene, LoadSceneMode mode)
        {
            if (FightManager.Instance == null) return;
            if (_buttonsAdded) return;
            _buttonsAdded = true;
            ArenaInitializer.Instance.OnArenaInitialized += AddButtons;
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

        void AddButtons()
        {
            _buttonContainer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("commandButtonsContainer");

            _otherFoldout = new()
            {
                text = "Other",
                value = false
            };
            _buttonContainer.Add(_otherFoldout);

            AddPawnButtons();
            AddCreatureButtons();
            AddOtherButtons();
            AddHeroButtons();
            AddEnemyButtons();
            AddPickupButtons();
            AddArmorButtons();
            AddKillPlayerArmyButton();
            AddKillAllOpponentsButton();
            AddGoldButton();
            AddTimeScaleButtons();
        }

        void AddPawnButtons()
        {
            Foldout foldout = new()
            {
                text = "Player Units",
                value = false
            };
            _buttonContainer.Add(foldout);
            List<Unit> allUnits = new(_unitDatabase.GetAllPawns());
            allUnits.Add(_unitDatabase.Peasant);

            foreach (Unit u in allUnits)
            {
                Button b = new() { text = $"Spawn {u.name}" };
                b.clickable.clicked += () =>
                {
                    Unit p = Instantiate(u);
                    p.InitializeFight(0);
                    HeroManager.Instance.Hero.AddArmy(p);
                    FightManager.Instance.SpawnPlayerUnit(p);
                };
                foldout.Add(b);
            }
        }

        void AddCreatureButtons()
        {
            // Foldout creatureFoldout = new()
            // {
            //     text = "Creatures",
            //     value = false
            // };
            // _buttonContainer.Add(creatureFoldout);
            //
            // List<string> choices = new()
            // {
            //     "-----"
            // };
            // choices.AddRange(_unitDatabase.AllCreatures.ConvertAll(x => x.name));
            // DropdownField dropDownLeft = new("Team 0", choices, 0);
            // creatureFoldout.Add(dropDownLeft);
            //
            // List<string> oppChoices = new()
            // {
            //     "-----"
            // };
            // oppChoices.AddRange(_unitDatabase.GetAllOpponentCreatures().ConvertAll(x => x.name));
            // DropdownField dropDownRight = new("Team 1", oppChoices, 0);
            // creatureFoldout.Add(dropDownRight);
            //
            // Button b = new() { text = "Spawn" };
            // b.clickable.clicked += () =>
            // {
            //     // try to get creature from string name
            //     Creature cLeft = _unitDatabase.AllCreatures.Find(x => x.name == dropDownLeft.value);
            //     if (cLeft != null)
            //     {
            //         Creature instance = Instantiate(cLeft);
            //         FightManager.Instance.SpawnPlayerUnit(instance);
            //     }
            //
            //     Creature cRight = _unitDatabase.GetAllOpponentCreatures()
            //         .Find(x => x.name == dropDownRight.value);
            //     if (cRight != null)
            //     {
            //         Creature instance = Instantiate(cRight);
            //         FightManager.Instance.SpawnEnemyUnit(instance);
            //     }
            // };
            // creatureFoldout.Add(b);
            //
            // Button levelUpButton = new() { text = "Level Up" };
            // levelUpButton.clickable.clicked += () => { HeroManager.Instance.Hero.Army.ForEach(x => x.LevelUp()); };
            // creatureFoldout.Add(levelUpButton);
            //
            // Button spawnAllFriendly = new() { text = "Spawn All Friendly" };
            // spawnAllFriendly.clickable.clicked += () =>
            // {
            //     foreach (Creature c in _unitDatabase.AllCreatures)
            //     {
            //         Creature instance = Instantiate(c);
            //         FightManager.Instance.SpawnPlayerUnit(instance);
            //     }
            // };
            // creatureFoldout.Add(spawnAllFriendly);
            //
            // Button spawnAllHostile = new() { text = "Spawn All Hostile" };
            // spawnAllHostile.clickable.clicked += () =>
            // {
            //     foreach (Creature c in _unitDatabase.GetAllOpponentCreatures())
            //     {
            //         Creature instance = Instantiate(c);
            //         FightManager.Instance.SpawnEnemyUnit(instance);
            //     }
            // };
            // creatureFoldout.Add(spawnAllHostile);
        }

        void AddKillPlayerArmyButton()
        {
            Button b = new() { text = "Kill Player Army" };
            b.clickable.clicked += () =>
            {
                List<UnitController> playerUnits = new(FightManager.Instance.PlayerUnits);

                foreach (UnitController uc in playerUnits)
                {
                    if (uc is HeroController)
                        continue;
                    uc.Die();
                }
            };
            _buttonContainer.Add(b);
        }


        void AddKillAllOpponentsButton()
        {
            Button b = new() { text = "Kill All Opponents" };
            b.clickable.clicked += () =>
            {
                List<UnitController> enemyUnits = new(FightManager.Instance.EnemyUnits);

                foreach (UnitController uc in enemyUnits)
                    uc.Die();
            };
            _buttonContainer.Add(b);
        }


        void AddOtherButtons()
        {
            Button spawnExpOrbButton = new() { text = "Spawn Exp Orb" };
            spawnExpOrbButton.clickable.clicked += () =>
            {
                FightManager.Instance.GetComponent<PickupManager>().SpawnBunchExpStones();
            };
            _otherFoldout.Add(spawnExpOrbButton);

            Button spawnVasesButton = new() { text = "Spawn Vases" };
            spawnVasesButton.clickable.clicked += () =>
            {
                FightManager.Instance.GetComponent<BreakableVaseManager>().SpawnVases();
            };
            _otherFoldout.Add(spawnVasesButton);

            Button unlockBarracks = new() { text = "Unlock Barracks" };
            unlockBarracks.clickable.clicked += () => { FightManager.Instance.Campaign.Barracks.Unlock(); };
            _otherFoldout.Add(unlockBarracks);
        }

        void AddHeroButtons()
        {
            // list of abilities
            Foldout heroFoldout = new()
            {
                text = "Hero",
                value = false
            };
            _buttonContainer.Add(heroFoldout);

            List<string> choices = new()
            {
                "-----"
            };
            choices.AddRange(_unitDatabase.GetAllAbilities().ConvertAll(x => x.name));
            DropdownField abilityDropDown = new("Abilities", choices, 0);
            heroFoldout.Add(abilityDropDown);

            Button b = new() { text = "Add Ability" };
            b.clickable.clicked += () =>
            {
                Ability a = _unitDatabase.GetAllAbilities().Find(x => x.name == abilityDropDown.value);
                if (a != null)
                {
                    Ability instance = Instantiate(a);
                    HeroManager.Instance.Hero.AddAbility(instance);
                }
            };
            heroFoldout.Add(b);

            // remove all abilities from hero
            Button removeAbilitiesButton = new() { text = "Remove All Abilities" };
            removeAbilitiesButton.clickable.clicked += () =>
            {
                foreach (Ability a in HeroManager.Instance.Hero.GetAllAbilities())
                    HeroManager.Instance.Hero.RemoveAbility(a);
            };
            heroFoldout.Add(removeAbilitiesButton);
            // level up all abilities
            Button levelUpAbilitiesButton = new() { text = "Level Up All Abilities" };
            levelUpAbilitiesButton.clickable.clicked += () =>
            {
                foreach (Ability a in HeroManager.Instance.Hero.GetAllAbilities())
                    a.LevelUp();
            };
            heroFoldout.Add(levelUpAbilitiesButton);
            // start abilities
            Button startAbilitiesButton = new() { text = "Start All Abilities" };
            startAbilitiesButton.clickable.clicked += () =>
            {
                HeroManager.Instance.HeroController.StartAllAbilities();
            };
            heroFoldout.Add(startAbilitiesButton);
            // stop abilities
            Button stopAbilitiesButton = new() { text = "Stop All Abilities" };
            stopAbilitiesButton.clickable.clicked += () => { HeroManager.Instance.HeroController.StopAllAbilities(); };
            heroFoldout.Add(stopAbilitiesButton);
        }

        void AddEnemyButtons()
        {
            Foldout enemyFoldout = new()
            {
                text = "Enemies",
                value = false
            };
            _buttonContainer.Add(enemyFoldout);

            List<string> choices = new()
            {
                "-----"
            };
            choices.AddRange(_unitDatabase.GetAllEnemies().ConvertAll(x => x.name));
            DropdownField enemyDropDown = new("Enemies", choices, 0);
            enemyFoldout.Add(enemyDropDown);

            Button b = new() { text = "Spawn Enemy" };
            b.clickable.clicked += () =>
            {
                Enemy e = _unitDatabase.GetAllEnemies().Find(x => x.name == enemyDropDown.value);
                if (e != null) SpawnEnemy(e);
            };
            enemyFoldout.Add(b);

            Button spawnFive = new() { text = "Spawn 5 Enemies" };
            spawnFive.clickable.clicked += () =>
            {
                Enemy e = _unitDatabase.GetAllEnemies().Find(x => x.name == enemyDropDown.value);
                if (e != null)
                    for (int i = 0; i < 5; i++)
                        SpawnEnemy(e);
            };
            enemyFoldout.Add(spawnFive);

            Button spawnAllEnemiesButton = new() { text = "Spawn All Enemies" };
            spawnAllEnemiesButton.clickable.clicked += () =>
            {
                foreach (Enemy e in _unitDatabase.GetAllEnemies()) SpawnEnemy(e);
            };
        }

        void AddPickupButtons()
        {
            Foldout pickupFoldout = new()
            {
                text = "Pickups",
                value = false
            };
            _buttonContainer.Add(pickupFoldout);

            foreach (Pickup p in _gameManager.GameDatabase.GetAllPickups())
            {
                Button b = new() { text = $"Spawn {p.name}" };
                b.clickable.clicked += () =>
                {
                    Pickup instance = Instantiate(p);
                    FightManager.Instance.GetComponent<PickupManager>().SpawnPickup(instance, Vector3.zero);
                };
                pickupFoldout.Add(b);
            }
        }

        void AddArmorButtons()
        {
            UnitDatabase unitDatabase = _gameManager.UnitDatabase;

            Foldout armorFoldout = new()
            {
                text = "Armor",
                value = false
            };
            _buttonContainer.Add(armorFoldout);

            List<Item> allArmors;
            if (HeroManager.Instance == null) return;
            Hero hero = HeroManager.Instance.Hero;

            if (hero.VisualHero.BodyType == 0)
                allArmors = new(unitDatabase.GetAllFemaleHeroArmor());
            else
                allArmors = new(unitDatabase.GetAllMaleHeroArmor());

            List<string> choices = new()
            {
                "-----"
            };
            choices.AddRange(allArmors.ConvertAll(x => x.name));
            DropdownField armorDropDown = new("Armor", choices, 0);
            armorFoldout.Add(armorDropDown);

            Button b = new() { text = "Add Armor" };
            b.clickable.clicked += () =>
            {
                Armor a = allArmors.Find(x => x.name == armorDropDown.value) as Armor;
                if (a != null)
                    hero.AddArmor(a);
            };
            armorFoldout.Add(b);
        }

        void AddGoldButton()
        {
            Button goldButton = new() { text = "Add 10k gold" };
            goldButton.clickable.clicked += () => { _gameManager.ChangeGoldValue(10000); };
            _buttonContainer.Add(goldButton);
        }

        void AddTimeScaleButtons()
        {
            VisualElement c = new();
            c.Add(new Label("Game speed: "));

            VisualElement container = new();
            c.Add(container);
            container.style.flexDirection = FlexDirection.Row;
            _buttonContainer.Add(c);

            for (int i = 0; i < 5; i++)
            {
                Button b = new() { text = $"{i + 1}x" };
                int j = i;
                b.clickable.clicked += () => { Time.timeScale = j + 1; };
                container.Add(b);
            }
        }

        void SpawnEnemy(Enemy e)
        {
            UnitController c = FightManager.Instance.SpawnEnemyUnit(e.Id);
            c.OnFightStarted();
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
            if (_commandTextField.text.ToLower() == "tween")
                DoTweenSeeAllTweens();
        }

        void DoTweenSeeAllTweens()
        {
            foreach (Tween tween in DOTween.PlayingTweens())
                Debug.Log($"{tween} is tweening | tween target: {tween.target}");
        }

        void Log(string logString, string stackTrace, LogType type)
        {
            if (_logContainer == null) return;
            _logContainer.Add(new Label(logString));

            _myLog = logString + "\n" + _myLog;
            if (_myLog.Length > 5000)
                _myLog = _myLog.Substring(0, 4000);

            FileManager.WriteToFile("log", _myLog);
        }
    }
}