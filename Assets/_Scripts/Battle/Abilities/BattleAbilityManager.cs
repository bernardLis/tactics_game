using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using DG.Tweening;

public class BattleAbilityManager : MonoBehaviour
{
    GameManager _gameManager;
    VisualElement _root;

    PlayerInput _playerInput;

    List<Ability> _abilities = new();

    Ability _selectedAbility;
    AbilityExecutor _abilityExecutor;

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
        _abilities = _gameManager.HeroDatabase.GetAllAbilities();

        foreach (Ability ability in _abilities)
        {
            AbilityButton button = new(ability);
            button.RegisterCallback<PointerUpEvent>(e => HighlightAbilityArea(ability));
            bottomPanel.Add(button);
        }
    }

    void ButtonOneClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        HighlightAbilityArea(_abilities[0]);
    }

    void ButtonTwoClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        HighlightAbilityArea(_abilities[1]);
    }

    void ButtonThreeClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        HighlightAbilityArea(_abilities[2]);
    }

    void ButtonFourClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        HighlightAbilityArea(_abilities[3]);
    }

    void HighlightAbilityArea(Ability ability)
    {
        if (_selectedAbility != null) CancelAbility();
        IsAbilitySelected = true;

        _selectedAbility = ability;
        _abilityExecutor = Instantiate(_selectedAbility.AbilityExecutorPrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<AbilityExecutor>();
        _abilityExecutor.HighlightAbilityArea();
    }

    void LeftMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (this == null) return;
        if (_abilityExecutor == null) return;

        _abilityExecutor.ExecuteAbility(_selectedAbility);
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
        IsAbilitySelected = false;
        _abilityExecutor.ClearAbilityHighlight();
        _abilityExecutor.CancelAbility();
        _selectedAbility = null;
    }
}
