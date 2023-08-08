using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BattleDeploymentManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;
    BattleManager _battleManager;

    [SerializeField] GameObject _creatureSpawnerPrefab;
    [SerializeField] GameObject _obstaclePrefab;

    GameObject _deployedObjectInstance;
    EntitySpawner _creatureSpawnerInstance;
    BattleGrabbableObstacle _obstacleInstance;
    BattleTurret _battleTurret;

    VisualElement _topPanel;
    Label _tooltipText;

    int _floorLayerMask;
    bool _wasDeployed;
    int posY;


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


    public void HandlePlayerArmyDeployment()
    {
        _wasDeployed = false;
        posY = 0;

        ShowTooltip("Click to deploy your army");

        _deployedObjectInstance = Instantiate(_creatureSpawnerPrefab);
        _creatureSpawnerInstance = _deployedObjectInstance.GetComponent<EntitySpawner>();
        _creatureSpawnerInstance.ShowPortal(_gameManager.PlayerHero.Element);
        StartCoroutine(UpdateObjectPosition());
    }

    public void HandleObstacleDeployment(Vector3 size)
    {
        _wasDeployed = false;
        posY = 3;

        ShowTooltip("Click to drop obstacle");

        _deployedObjectInstance = Instantiate(_obstaclePrefab);
        _obstacleInstance = _deployedObjectInstance.GetComponent<BattleGrabbableObstacle>();
        _obstacleInstance.Initialize(size);
        StartCoroutine(UpdateObjectPosition());
    }

    public void HandleTurretDeployment(Turret turret)
    {
        _wasDeployed = false;
        posY = 0;

        ShowTooltip("Click to deploy turret");

        _deployedObjectInstance = Instantiate(turret.Prefab);
        _battleTurret = _deployedObjectInstance.GetComponent<BattleTurret>();
        _battleTurret.Initialize(turret);
        StartCoroutine(UpdateObjectPosition());
    }

    void ShowTooltip(string text)
    {
        _tooltipText = new(text);
        _tooltipText.AddToClassList("common__text-primary");
        _tooltipText.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));
        _tooltipText.style.fontSize = 24;
        _topPanel.Add(_tooltipText);
    }

    IEnumerator UpdateObjectPosition()
    {
        while (_deployedObjectInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new(hit.point.x, posY, hit.point.z);
                _deployedObjectInstance.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (this == null) return;
        if (!_battleManager.IsTimerOn) return;

        if (_wasDeployed) return;
        _wasDeployed = true;

        if (_tooltipText.parent != null)
            _topPanel.Remove(_tooltipText);

        if (_creatureSpawnerInstance != null) DeployArmy();
        if (_obstacleInstance != null) PlaceObstacle();
        if (_battleTurret != null) PlaceTurret();

        StopAllCoroutines();
    }

    void DeployArmy()
    {
        _creatureSpawnerInstance.SpawnHeroArmy(_gameManager.PlayerHero);
        _creatureSpawnerInstance.OnSpawnComplete += (list) =>
        {
            _battleManager.AddPlayerArmyEntities(list);
            _creatureSpawnerInstance.DestroySelf();
            OnPlayerArmyDeployed?.Invoke();
            _creatureSpawnerInstance = null;
        };
    }

    void PlaceObstacle()
    {
        _obstacleInstance.GetComponent<Rigidbody>().isKinematic = false;
        _obstacleInstance = null;
    }

    void PlaceTurret()
    {
        _battleTurret.StartTurretCoroutine();
        _battleTurret = null;
    }

}
