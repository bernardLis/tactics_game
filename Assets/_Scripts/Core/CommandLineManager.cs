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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Lis.Core
{
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
        [SerializeField] ExperienceStone[] _expOrbs;

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
                    AddCreature(instance, 0, Vector3.zero);
                }

                Creature cRight = _allCreatures.Find(x => x.name == dropDownRight.value);
                if (cRight != null)
                {
                    Creature instance = Instantiate(cRight);
                    AddCreature(instance, 1, Vector3.zero);
                }
            };
            creatureFoldout.Add(b);

            Button levelUpButton = new() { text = "Level Up" };
            levelUpButton.clickable.clicked += () =>
            {
                foreach (UnitController e in BattleManager.Instance.PlayerEntities)
                {
                    CreatureController bc = (CreatureController)e;
                    bc.Creature.LevelUp();
                }
            };
            creatureFoldout.Add(levelUpButton);

            Button spawnAllFriendly = new() { text = "Spawn All Friendly" };
            spawnAllFriendly.clickable.clicked += () =>
            {
                foreach (Creature c in _allCreatures)
                {
                    Creature instance = Instantiate(c);
                    AddCreature(instance, 0, GetMousePosition());
                }
            };
            creatureFoldout.Add(spawnAllFriendly);

            Button spawnAllHostile = new() { text = "Spawn All Hostile" };
            spawnAllHostile.clickable.clicked += () =>
            {
                foreach (Creature c in _allCreatures)
                {
                    Creature instance = Instantiate(c);
                    AddCreature(instance, 1, GetMousePosition());
                }
            };
            creatureFoldout.Add(spawnAllHostile);
        }

        void AddCreature(Creature c, int team, Vector3 pos)
        {
            c.InitializeBattle(team);
            UnitController be = SpawnEntity(c, team, pos);
            if (team == 0)
                BattleManager.Instance.AddPlayerArmyEntity(be);
            if (team == 1)
                BattleManager.Instance.AddOpponentArmyEntity(be);

            CreatureController bc = (CreatureController)be;
            bc.DebugInitialize(team);
        }

        void AddRandomCreature(int team)
        {
            Creature c = Instantiate(_allCreatures[Random.Range(0, _allCreatures.Count)]);
            c.InitializeBattle(team);
            UnitController be = SpawnEntity(c, team, Vector3.zero);
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
                    BattleManager.Instance.GetComponent<FightManager>().DebugSpawnMinion();
            };
            container.Add(input);
            container.Add(bMinions);
            minionFoldout.Add(container);
        }


        UnitController SpawnEntity(Unit unit, int team, Vector3 spawnPos)
        {
            Vector3 pos = spawnPos + new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
            GameObject instance = Instantiate(unit.Prefab, pos, transform.localRotation);
            UnitController be = instance.GetComponent<UnitController>();
            be.InitializeGameObject();
            be.InitializeEntity(unit, team);
            return be;
        }

        void AddClearButton()
        {
            BattleManager battleManager = BattleManager.Instance;

            Button clearButton = new() { text = "Clear" };
            clearButton.clickable.clicked += () =>
            {
                List<UnitController> collection = new(battleManager.PlayerEntities);
                collection.AddRange(battleManager.OpponentEntities);
                foreach (UnitController e in collection)
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
                BattleManager.Instance.GetComponent<PickupManager>().SpawnExpOrb(Vector3.zero);
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

            BattleManager battleManager = BattleManager.Instance;
            if (battleManager == null) return;
            Hero hero = battleManager.Hero;

            Button levelUpButton = new() { text = "Level up" };
            levelUpButton.clickable.clicked += () =>
            {
                hero = BattleManager.Instance.Hero;
                hero.AddExp(hero.GetExpForNextLevel() - hero.Experience.Value);
            };
            heroFoldout.Add(levelUpButton);

            Button increaseGatherStrength = new() { text = "Increase Pull" };
            increaseGatherStrength.clickable.clicked += () =>
            {
                hero = BattleManager.Instance.Hero;
                hero.Pull.SetBaseValue(hero.Pull.GetValue() + 1);
            };
            heroFoldout.Add(increaseGatherStrength);

            Button decreaseGatherStrength = new() { text = "Decrease Pull" };
            decreaseGatherStrength.clickable.clicked += () =>
            {
                hero = BattleManager.Instance.Hero;
                hero.Pull.SetBaseValue(hero.Pull.GetValue() - 1);
            };
            heroFoldout.Add(decreaseGatherStrength);

            List<string> abilityChoices = new();
            abilityChoices.AddRange(_gameManager.EntityDatabase.GetAllBasicAbilities().ConvertAll(x => x.name));
            var abilityDropdown = new DropdownField("Ability", abilityChoices, 0);
            Button b = new() { text = "Add Ability" };
            b.clickable.clicked += () =>
            {
                Ability a = _gameManager.EntityDatabase.GetAllBasicAbilities()
                    .Find(x => x.name == abilityDropdown.value);
                if (a != null)
                {
                    BattleManager.Instance.Hero.AddAbility(a);
                }
            };

            heroFoldout.Add(abilityDropdown);
            heroFoldout.Add(b);

            Button abilityLevelUp = new() { text = "Level up abilities" };
            abilityLevelUp.clickable.clicked += () =>
            {
                foreach (Ability a in BattleManager.Instance.Hero.Abilities)
                {
                    a.LevelUp();
                }
            };
            heroFoldout.Add(abilityLevelUp);

            // start abilities
            Button startAbilities = new() { text = "Start abilities" };
            startAbilities.clickable.clicked += () =>
            {
                foreach (Ability a in BattleManager.Instance.Hero.Abilities)
                {
                    a.StartAbility();
                }
            };
            heroFoldout.Add(startAbilities);

            // stop abilities
            Button stopAbilities = new() { text = "Stop abilities" };
            stopAbilities.clickable.clicked += () =>
            {
                foreach (Ability a in BattleManager.Instance.Hero.Abilities)
                {
                    a.StopAbility();
                }
            };
            heroFoldout.Add(stopAbilities);
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
            if (_commandTextField.text.ToLower() == "killbill")
                KillAllPlayerCreatures();
            if (_commandTextField.text.ToLower() == "tween")
                DoTweenSeeAllTweens();
        }

        void KillAllPlayerCreatures()
        {
            BattleManager battleManager = BattleManager.Instance;
            if (battleManager == null) return;
            List<UnitController> creatures = new(battleManager.PlayerEntities);
            foreach (CreatureController creature in creatures)
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

        Camera _cam;

        Vector3 GetMousePosition()
        {
            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return Vector3.zero;
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                return Vector3.zero;
            return hit.point;
        }
    }
}