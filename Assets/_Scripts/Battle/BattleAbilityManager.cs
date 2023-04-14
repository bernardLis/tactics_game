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
    BattleAbilityArea _selectedAbilityArea;


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
        VisualElement abilityContainer = _root.Q<VisualElement>("AbilityContainer");
        _abilities = _gameManager.HeroDatabase.GetAllAbilities();

        foreach (Ability ability in _abilities)
        {
            AbilityButton button = new(ability);
            button.RegisterCallback<PointerUpEvent>(e => HighlightAbilityArea(ability));
            abilityContainer.Add(button);
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
        if (_selectedAbility != null) CancelAbilityHighlight();

        _selectedAbility = ability;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        GameObject instance = Instantiate(_selectedAbility.AreaHighlightPrefab, worldPos, Quaternion.identity);
        _selectedAbilityArea = instance.GetComponent<BattleAbilityArea>();
    }

    void LeftMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (this == null) return;

        StartCoroutine(ExecuteAbility());
    }

    void RightMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (this == null) return;

        CancelAbilityHighlight();
    }

    // TODO: this should per ability...
    IEnumerator ExecuteAbility()
    {
        if (_selectedAbility == null) yield break;


        GameObject instance = Instantiate(_selectedAbility.EffectPrefab, _selectedAbilityArea.transform.position, Quaternion.identity);
        List<BattleEntity> entities = new(_selectedAbilityArea.GetEntitiesInArea());

        _selectedAbilityArea.IsHighlightingEnabled = false;
        _selectedAbilityArea.ClearHighlightedEntities();
        CancelAbilityHighlight();

        if (_selectedAbility == _abilities[0])
            yield return ExecuteFireball(instance, entities);
        if (_selectedAbility == _abilities[1])
            yield return ExecuteFreeze(instance, entities);

    }

    IEnumerator ExecuteFireball(GameObject effectInstance, List<BattleEntity> entities)
    {
        foreach (BattleEntity entity in entities)
            StartCoroutine(entity.GetHit(null, _selectedAbility));

        yield return new WaitForSeconds(3f);
        yield return effectInstance.transform.DOScale(0f, 1f).WaitForCompletion();
        Destroy(effectInstance);
    }

    IEnumerator ExecuteFreeze(GameObject effectInstance, List<BattleEntity> entities)
    {
        // TODO: freeze entities
        // spawn effect on each entity
        // prevent them from moving for 2 seconds

        yield return null;
    }

    void CancelAbilityHighlight()
    {
        if (_selectedAbility == null) return;
        _selectedAbility = null;

        if (_selectedAbilityArea == null) return;
        Destroy(_selectedAbilityArea.gameObject);
    }

}
