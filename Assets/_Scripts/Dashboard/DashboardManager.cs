using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DashboardManager : Singleton<DashboardManager>
{
    GameManager _gameManager;

    public VisualElement Root { get; private set; }

    // resources
    Label _navDay;
    VisualElement _navGold;
    GoldElement _goldElement;
    VisualElement _navTroops;
    VisualElement _navSpiceYellow;
    VisualElement _navSpiceBlue;
    VisualElement _navSpiceRed;

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
        _navSpiceYellow = Root.Q<VisualElement>("navSpiceYellow");
        _navSpiceBlue = Root.Q<VisualElement>("navSpiceBlue");
        _navSpiceRed = Root.Q<VisualElement>("navSpiceRed");

        _main = Root.Q<VisualElement>("main");
        _mainArmory = Root.Q<VisualElement>("mainArmory");
        _mainShop = Root.Q<VisualElement>("mainShop");
        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _mainCamp = Root.Q<VisualElement>("mainCamp");
        _mainAbilities = Root.Q<VisualElement>("mainAbilities");

        _mainExit = Root.Q<VisualElement>("mainExit");
        _mainExit.RegisterCallback<PointerUpEvent>(HideMain);

        UpdateDay(_gameManager.Day);
        AddGoldElement();
        AddTroopsElement();
        AddSpiceElements();
    }

    void Start()
    {
        _dashboardPlayer = GameObject.FindObjectOfType<DashboardPlayer>();
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

    public void ShowDeskUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;
        BaseBuildingOpened();

        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskOpened?.Invoke();
    }

    public void ShowArmoryUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainArmory.style.display = DisplayStyle.Flex;
        OnArmoryOpened?.Invoke();
    }

    public void ShowAbilitiesUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainAbilities.style.display = DisplayStyle.Flex;
        OnAbilitiesOpened?.Invoke();
    }

    public void ShowShopUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        BaseBuildingOpened();

        _mainShop.style.display = DisplayStyle.Flex;
        OnShopOpened?.Invoke();

    }

    public void ShowCampUI(InputAction.CallbackContext ctx)
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
        _main.style.display = DisplayStyle.Flex;
        HideAllPanels();
        if (_dashboardPlayer != null)
            _dashboardPlayer.SetInputAllowed(false);
    }

    void HideAllPanels()
    {
        _mainArmory.style.display = DisplayStyle.None;
        _mainShop.style.display = DisplayStyle.None;
        _mainDesk.style.display = DisplayStyle.None;
        _mainCamp.style.display = DisplayStyle.None;
        _mainAbilities.style.display = DisplayStyle.None;

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
        _navSpiceYellow.Clear();
        _navSpiceBlue.Clear();
        _navSpiceRed.Clear();

        SpiceElement y = new(_gameManager.YellowSpice, SpiceColor.Yellow);
        _gameManager.OnYellowSpiceChanged += y.OnValueChanged;
        _navSpiceYellow.Add(y);

        SpiceElement b = new(_gameManager.BlueSpice, SpiceColor.Blue);
        _gameManager.OnBlueSpiceChanged += b.OnValueChanged;
        _navSpiceBlue.Add(b);

        SpiceElement r = new(_gameManager.RedSpice, SpiceColor.Red);
        _gameManager.OnRedSpiceChanged += r.OnValueChanged;
        _navSpiceRed.Add(r);
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

    [ContextMenu("Add 500 yellow spice")]
    void Add500YellowSpice()
    {
        _gameManager.ChangeGoldValue(10000);
    }

    [ContextMenu("Add 500 blue spice")]
    void Add500BlueSpice()
    {
        _gameManager.ChangeGoldValue(10000);
    }
    [ContextMenu("Add 500 red spice")]
    void Add500RedSpice()
    {
        _gameManager.ChangeGoldValue(10000);
    }



#endif

}
