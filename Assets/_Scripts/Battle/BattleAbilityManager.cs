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

    [SerializeField] GameObject _abilityAreaPrefab;

    Ability _selectedAbility;
    BattleAbilityArea _selectedAbilityArea;
    [SerializeField] GameObject _effect;


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

    void ButtonOneClick(InputAction.CallbackContext context) { HighlightAbilityArea(_abilities[0]); }

    void ButtonTwoClick(InputAction.CallbackContext context) { HighlightAbilityArea(_abilities[1]); }

    void ButtonThreeClick(InputAction.CallbackContext context) { HighlightAbilityArea(_abilities[2]); }

    void ButtonFourClick(InputAction.CallbackContext context) { HighlightAbilityArea(_abilities[3]); }

    void HighlightAbilityArea(Ability ability)
    {
        if (_selectedAbility != null) CancelAbilityHighlight();

        Debug.Log($"click click highlight ");
        _selectedAbility = ability;
        Debug.Log($"Using {ability.name}");
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        GameObject instance = Instantiate(_abilityAreaPrefab, worldPos, Quaternion.identity);
        _selectedAbilityArea = instance.GetComponent<BattleAbilityArea>();
    }

    void LeftMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        StartCoroutine(ExecuteAbility());
    }

    void RightMouseClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        CancelAbilityHighlight();
    }

    IEnumerator ExecuteAbility()
    {
        if (_selectedAbility == null) yield break;

        GameObject instance = Instantiate(_effect, _selectedAbilityArea.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        List<BattleEntity> entities = new(_selectedAbilityArea.GetEntitiesInArea());
        foreach (BattleEntity entity in entities)
        {
            StartCoroutine(entity.GetHit(null, 20));
        }
        CancelAbilityHighlight();
        yield return new WaitForSeconds(3f);
        yield return instance.transform.DOScale(0f, 1f).WaitForCompletion();
        Destroy(instance);

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
