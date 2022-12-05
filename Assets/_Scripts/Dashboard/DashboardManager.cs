using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DashboardManager : Singleton<DashboardManager>
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    public VisualElement Root { get; private set; }

    // resources
    Label _navDay;
    VisualElement _navGold;
    GoldElement _goldElement;
    VisualElement _navTroops;
    VisualElement _navSpice;

    TroopsLimitVisualElement _troopsLimitVisualElement;

    VisualElement _main;
    VisualElement _mainArmory;
    VisualElement _mainShop;
    VisualElement _mainDesk;
    VisualElement _mainCamp;
    VisualElement _mainAbilities;

    VisualElement _mainExit;

    VisualElement _activeNavTab;

    DashboardPlayer _dashboardPlayer;

    public event Action OnDeskOpened;
    public event Action OnArmoryOpened;
    public event Action OnAbilitiesOpened;
    public event Action OnShopOpened;
    public event Action OnCampOpened;
    public event Action OnHideAllPanels;

    protected override void Awake()
    {
        base.Awake();
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += UpdateDay;

        Root = GetComponent<UIDocument>().rootVisualElement;
        // resources
        _navDay = Root.Q<Label>("navDayLabel");
        _navGold = Root.Q<VisualElement>("navGold");
        _navTroops = Root.Q<VisualElement>("navTroops");
        _navSpice = Root.Q<VisualElement>("navSpice");

        _main = Root.Q<VisualElement>("main");
        _mainArmory = Root.Q<VisualElement>("mainArmory");
        _mainShop = Root.Q<VisualElement>("mainShop");
        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _mainCamp = Root.Q<VisualElement>("mainCamp");
        _mainAbilities = Root.Q<VisualElement>("mainAbilities");

        _mainExit = Root.Q<VisualElement>("mainExit");
        _mainExit.RegisterCallback<PointerUpEvent>(HideMain);

        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        UpdateDay(_gameManager.Day);
        AddGoldElement();
        AddTroopsElement();
        AddSpiceElements();
    }

    void Start()
    {
        _dashboardPlayer = GameObject.FindObjectOfType<DashboardPlayer>();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["OpenDesk"].performed += ShowDeskUI;
        _playerInput.actions["OpenArmory"].performed += ShowArmoryUI;
        _playerInput.actions["OpenShop"].performed += ShowShopUI;
        _playerInput.actions["OpenCamp"].performed += ShowCampUI;
        _playerInput.actions["OpenAbilities"].performed += ShowAbilitiesUI;

        _playerInput.actions["CloseCurrentTab"].performed += HideAllPanels;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["OpenDesk"].performed -= ShowDeskUI;
        _playerInput.actions["OpenArmory"].performed -= ShowArmoryUI;
        _playerInput.actions["OpenShop"].performed -= ShowShopUI;
        _playerInput.actions["OpenCamp"].performed -= ShowCampUI;
        _playerInput.actions["OpenAbilities"].performed -= ShowAbilitiesUI;

        _playerInput.actions["CloseCurrentTab"].performed -= HideAllPanels;
    }

    void OnEnable()
    {
        // inputs
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Dashboard");

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    public void OpenDashboardBuilding(DashboardBuildingType db)
    {
        // new InputAction.CallbackContext() - coz it is hooked up in editor to open ui with f keys
        InputAction.CallbackContext a = new InputAction.CallbackContext();

        if (db == DashboardBuildingType.Desk)
            ShowDeskUI(a);
        if (db == DashboardBuildingType.Armory)
            ShowArmoryUI(a);
        if (db == DashboardBuildingType.Shop)
            ShowShopUI(a);
        if (db == DashboardBuildingType.Camp)
            ShowCampUI(a);
        if (db == DashboardBuildingType.Abilities)
            ShowAbilitiesUI(a);
    }

    void ShowDeskUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;
        BaseBuildingOpened();

        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskOpened?.Invoke();
    }

    void ShowArmoryUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainArmory.style.display = DisplayStyle.Flex;
        OnArmoryOpened?.Invoke();
    }

    void ShowAbilitiesUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainAbilities.style.display = DisplayStyle.Flex;
        OnAbilitiesOpened?.Invoke();
    }

    void ShowShopUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainShop.style.display = DisplayStyle.Flex;
        OnShopOpened?.Invoke();

    }

    void ShowCampUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainCamp.style.display = DisplayStyle.Flex;
        OnCampOpened?.Invoke();
    }

    bool IsValidAction(InputAction.CallbackContext ctx)
    {
        // otherwise it triggers 3 times: https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/
        // disabled is for my empty event action.

        if (ctx.performed || ctx.phase == InputActionPhase.Disabled)
            return true;
        return false;
    }

    void BaseBuildingOpened()
    {
        HideAllPanels();
        
        _main.style.display = DisplayStyle.Flex;
        _mainExit.style.display = DisplayStyle.Flex;
        if (_dashboardPlayer != null)
            _dashboardPlayer.SetInputAllowed(false);
    }

    void HideAllPanels(InputAction.CallbackContext ctx) { HideAllPanels(); }
    void HideAllPanels()
    {
        _mainArmory.style.display = DisplayStyle.None;
        _mainShop.style.display = DisplayStyle.None;
        _mainDesk.style.display = DisplayStyle.None;
        _mainCamp.style.display = DisplayStyle.None;
        _mainAbilities.style.display = DisplayStyle.None;

        _mainExit.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
    }

    void HideMain(PointerUpEvent e)
    {
        _main.style.display = DisplayStyle.None;
        if (_dashboardPlayer != null)
            _dashboardPlayer.SetInputAllowed(true);
    }

    void UpdateDay(int day) { _navDay.text = $"Day: {day}"; }

    void AddGoldElement()
    {
        _navGold.Clear();
        _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += _goldElement.ChangeAmount;
        _navGold.Add(_goldElement);
    }

    void AddTroopsElement()
    {
        _navTroops.Clear();
        _troopsLimitVisualElement = new();
        _navTroops.Add(_troopsLimitVisualElement);
    }

    void AddSpiceElements()
    {
        _navSpice.Clear();

        SpiceElement y = new(_gameManager.Spice);
        _gameManager.OnSpiceChanged += y.OnValueChanged;
        _navSpice.Add(y);
    }

#if UNITY_EDITOR

    [ContextMenu("Add 10000 Gold")]
    void Add100Gold()
    {
        _gameManager.ChangeGoldValue(10000);
    }

    [ContextMenu("Remove 5000 Gold")]
    void Remove50Gold()
    {
        _gameManager.ChangeGoldValue(-5000);
    }

    [ContextMenu("Add 500 spice")]
    void Add500Spice()
    {
        _gameManager.ChangeSpiceValue(500);
    }

#endif

}
