using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using DG.Tweening;

public class BattleAbilityManager : MonoBehaviour
{
    const string _ussClassName = "battle__";
    const string _ussAbilityContainer = _ussClassName + "ability-container";


    GameManager _gameManager;
    VisualElement _root;

    PlayerInput _playerInput;

    List<Ability> _abilities = new();

    Ability _selectedAbility;
    AbilityExecutor _abilityExecutor;

    Hero _hero;
    List<AbilityButton> _abilityButtons = new();

    public bool IsAbilitySelected { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        AddAbilityButtons();
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Battle");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["1"].performed += ButtonOneClick;
        _playerInput.actions["2"].performed += ButtonTwoClick;
        _playerInput.actions["3"].performed += ButtonThreeClick;
        _playerInput.actions["4"].performed += ButtonFourClick;

        _playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
        _playerInput.actions["RightMouseClick"].performed += RightMouseClick;

    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["1"].performed -= ButtonOneClick;
        _playerInput.actions["2"].performed -= ButtonTwoClick;
        _playerInput.actions["3"].performed -= ButtonThreeClick;
        _playerInput.actions["4"].performed -= ButtonFourClick;

        _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
        _playerInput.actions["RightMouseClick"].performed -= RightMouseClick;
    }

    void AddAbilityButtons()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");
        _hero = _gameManager.SelectedBattle.Hero;
        _abilities = _hero.Abilities;

        VisualElement container = new();
        container.AddToClassList(_ussAbilityContainer);
        bottomPanel.Add(container);
        int i = 1;
        foreach (Ability ability in _abilities)
        {
            AbilityButton button = new(ability, i.ToString());
            button.RegisterCallback<PointerUpEvent>(e => HighlightAbilityArea(ability, button));
            container.Add(button);
            _abilityButtons.Add(button);
            i++;
        }
    }

    void ButtonOneClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HighlightAbilityArea(_abilities[0], _abilityButtons[0]);
    }

    void ButtonTwoClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HighlightAbilityArea(_abilities[1], _abilityButtons[1]);
    }

    void ButtonThreeClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HighlightAbilityArea(_abilities[2], _abilityButtons[2]);
    }

    void ButtonFourClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HighlightAbilityArea(_abilities[3], _abilityButtons[3]);
    }

    void HighlightAbilityArea(Ability ability, AbilityButton abilityButton)
    {
        if (_selectedAbility != null) CancelAbility();
        if (abilityButton.IsOnCooldown)
        {
            Helpers.DisplayTextOnElement(_root, abilityButton, "Cool down, mate!", Color.red);
            return;
        }
        if (_hero.CurrentMana.Value < ability.GetManaCost())
        {
            Helpers.DisplayTextOnElement(_root, abilityButton, "Not enough mana!", Color.red);
            return;
        }

        _hero.CurrentMana.ApplyChange(-ability.GetManaCost());

        abilityButton.Highlight();
        IsAbilitySelected = true;

        _selectedAbility = ability;
        _abilityExecutor = Instantiate(_selectedAbility.AbilityExecutorPrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityExecutor>();
        _abilityExecutor.HighlightAbilityArea(_selectedAbility);
    }

    void LeftMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (this == null) return;
        if (_abilityExecutor == null) return;

        foreach (AbilityButton b in _abilityButtons)
            b.ClearHighlight();

        _abilityExecutor.ExecuteAbility(_selectedAbility);
        _selectedAbility.StartCooldown();
        IsAbilitySelected = false;
    }

    void RightMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (this == null) return;
        CancelAbility();
    }

    void CancelAbility()
    {
        if (_abilityExecutor == null) return;

        foreach (AbilityButton b in _abilityButtons)
            b.ClearHighlight();

        IsAbilitySelected = false;
        _abilityExecutor.ClearAbilityHighlight();
        _abilityExecutor.CancelAbility();
        _selectedAbility = null;
    }
}
