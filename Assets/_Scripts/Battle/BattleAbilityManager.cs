using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class BattleAbilityManager : MonoBehaviour
{
    GameManager _gameManager;
    VisualElement _root;

    PlayerInput _playerInput;

    List<Ability> _abilities = new();


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
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["1"].performed -= ButtonOneClick;
        _playerInput.actions["2"].performed -= ButtonTwoClick;
        _playerInput.actions["3"].performed -= ButtonThreeClick;
        _playerInput.actions["4"].performed -= ButtonFourClick;
    }


    void AddAbilityButtons()
    {
        VisualElement abilityContainer = _root.Q<VisualElement>("AbilityContainer");
        _abilities = _gameManager.HeroDatabase.GetAllAbilities();

        foreach (Ability ability in _abilities)
        {
            AbilityButton button = new(ability);
            button.RegisterCallback<PointerUpEvent>(e => UseAbility(ability));
            abilityContainer.Add(button);
        }
    }

    void ButtonOneClick(InputAction.CallbackContext context) { UseAbility(_abilities[0]); }

    void ButtonTwoClick(InputAction.CallbackContext context) { UseAbility(_abilities[1]); }

    void ButtonThreeClick(InputAction.CallbackContext context) { UseAbility(_abilities[2]); }

    void ButtonFourClick(InputAction.CallbackContext context) { UseAbility(_abilities[3]); }

    void UseAbility(Ability ability)
    {
        Debug.Log($"Using {ability.name}");
    }

}
