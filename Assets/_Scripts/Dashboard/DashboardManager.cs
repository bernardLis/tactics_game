using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DashboardManager : Singleton<DashboardManager>
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    public VisualElement Root { get; private set; }

    MyButton _passDayButton;

    // resources
    Label _navDay;
    VisualElement _navGold;
    GoldElement _goldElement;
    VisualElement _navTroops;
    VisualElement _navSpice;

    TroopsLimitElement _troopsLimitVisualElement;

    VisualElement _main;
    VisualElement _mainDesk;
    VisualElement _mainCamp;
    VisualElement _mainAbilities;

    DashboardBuildingType _openBuilding = DashboardBuildingType.Desk;

    public event Action OnDeskOpened;
    public event Action OnCampOpened;
    public event Action OnAbilitiesOpened;
    public event Action OnHideAllPanels;

    protected override void Awake()
    {
        base.Awake();
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += UpdateDay;

        Root = GetComponent<UIDocument>().rootVisualElement;

        AddPassDayButton();

        // resources
        _navDay = Root.Q<Label>("navDayLabel");
        _navGold = Root.Q<VisualElement>("navGold");
        _navTroops = Root.Q<VisualElement>("navTroops");
        _navSpice = Root.Q<VisualElement>("navSpice");

        _main = Root.Q<VisualElement>("main");
        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _mainCamp = Root.Q<VisualElement>("mainCamp");
        _mainAbilities = Root.Q<VisualElement>("mainAbilities");

        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        UpdateDay(_gameManager.Day);
        AddGoldElement();
        AddTroopsElement();
        AddSpiceElements();

        ShowPassDayButton();
        AddNavigationButtons();
    }

    /* INPUT */
    void SubscribeInputActions()
    {
        _playerInput.actions["OpenDesk"].performed += ShowDeskUI;
        _playerInput.actions["OpenCamp"].performed += ShowCampUI;
        _playerInput.actions["OpenAbilities"].performed += ShowAbilitiesUI;

        _playerInput.actions["CloseCurrentTab"].performed += HideAllPanels;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["OpenDesk"].performed -= ShowDeskUI;
        _playerInput.actions["OpenCamp"].performed -= ShowCampUI;
        _playerInput.actions["OpenAbilities"].performed -= ShowAbilitiesUI;

        _playerInput.actions["CloseCurrentTab"].performed -= HideAllPanels;
    }

    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Dashboard");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }


    /* PASS DAY BUTTON */
    void AddPassDayButton()
    {
        _passDayButton = new("Pass Day", "passDayButton", PassDay);
        _passDayButton.AddToClassList("textPrimary");
        Root.Q<VisualElement>("navLeft").Add(_passDayButton);
        HidePassDay();
    }

    void PassDay() { _gameManager.PassDay(); }

    void ShowPassDayButton()
    {
        _passDayButton.style.display = DisplayStyle.Flex;
        DOTween.To(() => _passDayButton.style.opacity.value, x => _passDayButton.style.opacity = x, 1f, 1f);
    }

    void HidePassDay()
    {
        _passDayButton.style.display = DisplayStyle.None;
        _passDayButton.style.opacity = 0;
    }

    /* RESOURCES */
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

    /* NAVIGATION */
    void AddNavigationButtons()
    {
        VisualElement navRight = Root.Q<VisualElement>("navRight");
        MyButton navDesk = new(null, "navDesk", OpenDesk);
        MyButton navCamp = new(null, "navCamp", OpenCamp);
        MyButton navAbilities = new(null, "navAbilities", OpenAbilities);
        MyButton navArchive = new(null, "navArchive", OnArchiveClick);

        navDesk.AddToClassList("navIcon");
        navCamp.AddToClassList("navIcon");
        navAbilities.AddToClassList("navIcon");
        navArchive.AddToClassList("navIcon");

        navRight.Add(navDesk);
        navRight.Add(navCamp);
        navRight.Add(navAbilities);
        navRight.Add(navArchive);
    }

    void OpenDesk() { OpenDashboardBuilding(DashboardBuildingType.Desk); }
    void OpenCamp() { OpenDashboardBuilding(DashboardBuildingType.Camp); }
    void OpenAbilities() { OpenDashboardBuilding(DashboardBuildingType.Abilities); }

    void OnArchiveClick()
    {
        FullScreenElement visual = new FullScreenElement();
        visual.AddToClassList("textPrimary");
        visual.style.backgroundColor = Color.black;
        visual.style.left = Screen.width;

        ScrollView container = new();
        visual.Add(container);

        DOTween.To(x => visual.style.left = x, Screen.width, 0f, 1f);

        foreach (Report report in _gameManager.ReportsArchived)
        {
            Label r = new Label($"{report.ReportType}");
            container.Add(r);
            // https://forum.unity.com/threads/send-additional-parameters-to-callback.777029/
            r.RegisterCallback<PointerUpEvent, Report>(OnArchivedReportClick, report);
        }
        visual.Initialize(Root);
        visual.AddBackButton();
    }

    void OnArchivedReportClick(PointerUpEvent evt, Report report)
    {
        FullScreenElement visual = new FullScreenElement();
        visual.style.backgroundColor = Color.black;
        visual.Add(new ReportElement(visual, report));
        visual.Initialize(Root);
        visual.AddBackButton();
    }

    public void OpenDashboardBuilding(DashboardBuildingType db)
    {
        // new InputAction.CallbackContext() - coz it is hooked up in editor to open ui with f keys
        InputAction.CallbackContext a = new InputAction.CallbackContext();

        if (db == DashboardBuildingType.Desk)
            ShowDeskUI(a);
        if (db == DashboardBuildingType.Camp)
            ShowCampUI(a);
        if (db == DashboardBuildingType.Abilities)
            ShowAbilitiesUI(a);
    }

    void ShowDeskUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        if (_openBuilding == DashboardBuildingType.Desk)
            return;
        _openBuilding = DashboardBuildingType.Desk;

        BaseBuildingOpened();
        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskOpened?.Invoke();
    }

    void ShowAbilitiesUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        if (_openBuilding == DashboardBuildingType.Abilities)
            return;
        _openBuilding = DashboardBuildingType.Abilities;

        BaseBuildingOpened();

        _mainAbilities.style.display = DisplayStyle.Flex;
        OnAbilitiesOpened?.Invoke();
    }

    void ShowCampUI(InputAction.CallbackContext ctx)
    {
        if (!IsValidAction(ctx))
            return;

        if (_openBuilding == DashboardBuildingType.Camp)
            return;
        _openBuilding = DashboardBuildingType.Camp;

        BaseBuildingOpened();

        _mainCamp.style.display = DisplayStyle.Flex;
        OnCampOpened?.Invoke();
    }

    bool IsValidAction(InputAction.CallbackContext ctx)
    {
        if (ctx.time == 0)
            return true; // for buttons
        // otherwise it triggers 3 times: https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/
        // disabled is for my empty event action.
        if (!ctx.performed && ctx.phase == InputActionPhase.Canceled)
            return true;
        return false;
    }

    void BaseBuildingOpened()
    {
        HideAllPanels();
        _main.style.display = DisplayStyle.Flex;
    }

    void HideAllPanels(InputAction.CallbackContext ctx) { HideAllPanels(); }
    void HideAllPanels()
    {
        _mainDesk.style.display = DisplayStyle.None;
        _mainCamp.style.display = DisplayStyle.None;
        _mainAbilities.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
    }


#if UNITY_EDITOR

    [ContextMenu("Add 10000 Gold")]
    void Add100Gold() { _gameManager.ChangeGoldValue(10000); }

    [ContextMenu("Remove 5000 Gold")]
    void Remove50Gold() { _gameManager.ChangeGoldValue(-5000); }

    [ContextMenu("Add 500 spice")]
    void Add500Spice() { _gameManager.ChangeSpiceValue(500); }

    [ContextMenu("Level Up")]
    void LevelUpAllCharacters()
    {
        foreach (Character c in _gameManager.PlayerTroops)
            c.LevelUp();
    }
#endif

}
