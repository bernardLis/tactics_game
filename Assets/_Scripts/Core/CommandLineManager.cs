using System.Collections.Generic;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Pawn;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class CommandLineManager : MonoBehaviour
    {
        GameManager _gameManager;
        PlayerInput _playerInput;
        UnitDatabase _unitDatabase;

        VisualElement _commandLineContainer;
        TextField _commandTextField;
        Button _submitCommandButton;

        ScrollView _logContainer;
        static string _myLog = "";

        bool _isOpen;

        Label _fpsLabel;
        float _deltaTime;

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

            if (BattleManager.Instance == null) return;
            BattleManager.Instance.GetComponent<BattleInitializer>().OnBattleInitialized += AddButtons;
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

        void AddButtons()
        {
            _buttonContainer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("commandButtonsContainer");

            _otherFoldout = new Foldout()
            {
                text = "Other",
                value = false
            };
            _buttonContainer.Add(_otherFoldout);

            AddPawnButtons();
            AddCreatureButtons();
            AddOtherButtons();
            AddHeroButtons();
            AddKillPlayerArmyButton();
            AddKillAllOpponentsButton();
        }

        void AddPawnButtons()
        {
            Foldout foldout = new()
            {
                text = "Pawns",
                value = false
            };
            _buttonContainer.Add(foldout);

            Button b = new() { text = "Add Random Pawn To Army" };
            b.clickable.clicked += () =>
            {
                Pawn p = Instantiate(_unitDatabase.GetRandomPawn());
                p.InitializeBattle(0);
                HeroManager.Instance.Hero.AddArmy(p);
            };
            foldout.Add(b);
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
                "-----"
            };
            choices.AddRange(_unitDatabase.AllCreatures.ConvertAll(x => x.name));
            var dropDownLeft = new DropdownField("Team 0", choices, 0);
            creatureFoldout.Add(dropDownLeft);

            List<string> oppChoices = new()
            {
                "-----"
            };
            oppChoices.AddRange(_unitDatabase.GetAllOpponentCreatures().ConvertAll(x => x.name));
            var dropDownRight = new DropdownField("Team 1", oppChoices, 0);
            creatureFoldout.Add(dropDownRight);

            Button b = new() { text = "Spawn" };
            b.clickable.clicked += () =>
            {
                // try to get creature from string name
                Creature cLeft = _unitDatabase.AllCreatures.Find(x => x.name == dropDownLeft.value);
                if (cLeft != null)
                {
                    Creature instance = Instantiate(cLeft);
                    FightManager.Instance.SpawnPlayerUnit(instance);
                }

                Creature cRight = _unitDatabase.GetAllOpponentCreatures()
                    .Find(x => x.name == dropDownRight.value);
                if (cRight != null)
                {
                    Creature instance = Instantiate(cRight);
                    FightManager.Instance.SpawnEnemyUnit(instance);
                }
            };
            creatureFoldout.Add(b);

            Button levelUpButton = new() { text = "Level Up" };
            levelUpButton.clickable.clicked += () => { HeroManager.Instance.Hero.Army.ForEach(x => x.LevelUp()); };
            creatureFoldout.Add(levelUpButton);

            Button spawnAllFriendly = new() { text = "Spawn All Friendly" };
            spawnAllFriendly.clickable.clicked += () =>
            {
                foreach (Creature c in _unitDatabase.AllCreatures)
                {
                    Creature instance = Instantiate(c);
                    FightManager.Instance.SpawnPlayerUnit(instance);
                }
            };
            creatureFoldout.Add(spawnAllFriendly);

            Button spawnAllHostile = new() { text = "Spawn All Hostile" };
            spawnAllHostile.clickable.clicked += () =>
            {
                foreach (Creature c in _unitDatabase.GetAllOpponentCreatures())
                {
                    Creature instance = Instantiate(c);
                    FightManager.Instance.SpawnEnemyUnit(instance);
                }
            };
            creatureFoldout.Add(spawnAllHostile);
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
                BattleManager.Instance.GetComponent<PickupManager>().SpawnBunchExpStones();
            };
            _otherFoldout.Add(spawnExpOrbButton);

            Button spawnVasesButton = new() { text = "Spawn Vases" };
            spawnVasesButton.clickable.clicked += () =>
            {
                BattleManager.Instance.GetComponent<BreakableVaseManager>().SpawnVases();
            };
            _otherFoldout.Add(spawnVasesButton);
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
            var abilityDropDown = new DropdownField("Abilities", choices, 0);
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
            {
                Debug.Log($"{tween} is tweening | tween target: {tween.target}");
            }
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