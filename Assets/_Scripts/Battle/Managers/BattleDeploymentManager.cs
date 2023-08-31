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
    BattleTooltipManager _tooltipManager;

    [SerializeField] GameObject _creatureSpawnerPrefab;
    [SerializeField] GameObject _obstaclePrefab;

    GameObject _deployedObjectInstance;
    EntitySpawner _creatureSpawnerInstance;
    BattleObstacle _obstacleInstance;
    BattleTurret _battleTurret;

    VisualElement _topPanel;
    //Label _tooltipText;

    int _floorLayerMask;
    bool _wasDeployed;
    int _posY;

    List<Creature> _creaturesToDeploy;

    public event Action OnPlayerArmyDeployed;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _floorLayerMask = LayerMask.GetMask("Floor");

        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

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

    public void HandlePlayerArmyDeployment(List<Creature> creaturesToDeploy)
    {
        BaseHandleDeployment("Click to deploy your army", 0);

        _creaturesToDeploy = creaturesToDeploy;

        _deployedObjectInstance = Instantiate(_creatureSpawnerPrefab);
        _creatureSpawnerInstance = _deployedObjectInstance.GetComponent<EntitySpawner>();
        _creatureSpawnerInstance.ShowPortal(_gameManager.PlayerHero.Element);
        StartCoroutine(UpdateObjectPosition());
    }

    public void HandleObstacleDeployment(Vector3 size)
    {
        BaseHandleDeployment("Click to deploy obstacle", 3);

        _deployedObjectInstance = Instantiate(_obstaclePrefab);
        _obstacleInstance = _deployedObjectInstance.GetComponent<BattleObstacle>();
        _obstacleInstance.Initialize(size);
        StartCoroutine(UpdateObjectPosition());
    }

    public void HandleTurretDeployment(Turret turret)
    {
        BaseHandleDeployment("Click to deploy turret", 0);

        _deployedObjectInstance = Instantiate(turret.Prefab);
        _battleTurret = _deployedObjectInstance.GetComponent<BattleTurret>();
        _battleTurret.Initialize(turret);
        StartCoroutine(UpdateObjectPosition());
    }

    void BaseHandleDeployment(string tooltip, int posY)
    {
        if (_deployedObjectInstance != null) Deploy();
        _wasDeployed = false;
        _posY = posY;
        ShowTooltip(tooltip);

        _creatureSpawnerInstance = null;
        _obstacleInstance = null;
        _battleTurret = null;

        BattleGrabManager.Instance.OnPointerUp(default);
        BattleAbilityManager.Instance.CancelAbility();
        BattleManager.BlockBattleInput = true;
    }

    void ShowTooltip(string text)
    {
        _tooltipManager.ShowInfo(new BattleInfoElement(text), true);
        /*
        _tooltipText = new(text);
        _tooltipText.AddToClassList("common__text-primary");
        _tooltipText.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));
        _tooltipText.style.fontSize = 24;
        _topPanel.Add(_tooltipText);
        */
    }

    IEnumerator UpdateObjectPosition()
    {
        while (_deployedObjectInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new(hit.point.x, _posY, hit.point.z);
                _deployedObjectInstance.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (this == null) return;
        if (!_battleManager.IsTimerOn) return;
        if (_deployedObjectInstance == null) return;

        _tooltipManager.RemoveInfoPriority();
        // if (_tooltipText.parent != null)
        //       _topPanel.Remove(_tooltipText);

        Deploy();
    }

    void Deploy()
    {
        if (_wasDeployed) return;
        _wasDeployed = true;

        if (_creatureSpawnerInstance != null) PlaceArmy(_creatureSpawnerInstance);
        if (_obstacleInstance != null) PlaceObstacle();
        if (_battleTurret != null) PlaceTurret();

        _deployedObjectInstance = null;
        StopAllCoroutines();
        BattleManager.BlockBattleInput = false;
    }

    void PlaceArmy(EntitySpawner spawner)
    {
        spawner.SpawnCreatures(_creaturesToDeploy);
        spawner.OnSpawnComplete += (list) =>
        {
            _battleManager.AddPlayerArmyEntities(list);
            spawner.DestroySelf();
            OnPlayerArmyDeployed?.Invoke();
            if (_creatureSpawnerInstance == spawner) _creatureSpawnerInstance = null;
        };
    }

    void PlaceObstacle()
    {
        _obstacleInstance.GetComponent<Rigidbody>().isKinematic = false;
        _obstacleInstance = null;
    }

    void PlaceTurret()
    {
        _battleManager.AddPlayerTurret(_battleTurret);
        _battleTurret.StartTurretCoroutine();
        _battleTurret = null;
    }

}
