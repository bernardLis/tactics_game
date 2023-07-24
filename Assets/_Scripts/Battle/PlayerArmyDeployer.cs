using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerArmyDeployer : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;
    BattleManager _battleManager;

    [SerializeField] GameObject _creatureSpawnerPrefab;

    VisualElement _topPanel;
    Label _tooltipText;

    int _floorLayerMask;
    bool _wasInitialized;
    EntitySpawner _creatureSpawnerInstance;
    bool _wasDeployed;

    public event Action OnPlayerArmyDeployed;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _floorLayerMask = LayerMask.GetMask("Floor");

        _battleManager = BattleManager.Instance;

        _topPanel = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("topPanel");
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
        _playerInput.actions["LeftMouseClick"].canceled += OnPointerUp;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
    }

    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (!_wasInitialized) return;
        if (this == null) return;
        if (_creatureSpawnerInstance == null) return;

        if (_wasDeployed) return;
        _wasDeployed = true;

        if (_tooltipText.parent != null)
            _topPanel.Remove(_tooltipText);

        _creatureSpawnerInstance.SpawnHeroArmy(_gameManager.PlayerHero);
        _creatureSpawnerInstance.OnSpawnComplete += (list) =>
        {
            _battleManager.AddPlayerArmyEntities(list);
            _creatureSpawnerInstance.DestroySelf();
            OnPlayerArmyDeployed?.Invoke();
        };
        StopAllCoroutines();
    }

    public void Initialize()
    {
        if (_wasInitialized) return;
        _wasInitialized = true;

        _tooltipText = new("Click to deploy your army");
        _tooltipText.AddToClassList("common__text-primary");
        _tooltipText.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));
        _tooltipText.style.fontSize = 24;
        _topPanel.Add(_tooltipText);

        GameObject instance = Instantiate(_creatureSpawnerPrefab);
        _creatureSpawnerInstance = instance.GetComponent<EntitySpawner>();
        _creatureSpawnerInstance.ShowPortal(_gameManager.PlayerHero.Element);
        StartCoroutine(UpdatePortalPosition());
    }

    IEnumerator UpdatePortalPosition()
    {
        while (_creatureSpawnerInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
                _creatureSpawnerInstance.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
