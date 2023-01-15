using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Threading.Tasks;

public class DashboardManager : Singleton<DashboardManager>
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    public VisualElement Root { get; private set; }

    public EffectHolder PanelOpenEffect;

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
    VisualElement _abilitiesWrapperLeft;
    VisualElement _abilitiesWrapperRight;

    DashboardBuildingType _openBuilding = DashboardBuildingType.Desk;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "dashboard__";
    const string _ussPassDayButton = _ussClassName + "pass-day-button";
    const string _ussNavIcon = _ussClassName + "nav-icon";
    const string _ussNavDesk = _ussClassName + "nav-desk";
    const string _ussNavCamp = _ussClassName + "nav-camp";
    const string _ussNavAbilities = _ussClassName + "nav-abilities";
    const string _ussNavArchive = _ussClassName + "nav-archive";

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
        _abilitiesWrapperLeft = Root.Q<VisualElement>("abilitiesWrapperLeft");
        _abilitiesWrapperRight = Root.Q<VisualElement>("abilitiesWrapperRight");


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
        _passDayButton = new("Pass Day", _ussPassDayButton, PassDay);
        _passDayButton.AddToClassList(_ussCommonTextPrimary);
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
        MyButton navDesk = new(null, _ussNavDesk, OpenDesk);
        MyButton navCamp = new(null, _ussNavCamp, OpenCamp);
        MyButton navAbilities = new(null, _ussNavAbilities, OpenAbilities);
        MyButton navArchive = new(null, _ussNavArchive, OnArchiveClick);

        navDesk.AddToClassList(_ussNavIcon);
        navCamp.AddToClassList(_ussNavIcon);
        navAbilities.AddToClassList(_ussNavIcon);
        navArchive.AddToClassList(_ussNavIcon);

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
        visual.AddToClassList(_ussCommonTextPrimary);
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

    async void ShowDeskUI(InputAction.CallbackContext ctx)
    {
        if (_openBuilding == DashboardBuildingType.Desk)
            return;
        if (_openBuilding == DashboardBuildingType.Camp)
            await HideCampUI();
        if (_openBuilding == DashboardBuildingType.Abilities)
            await HideAbilityUI();

        _openBuilding = DashboardBuildingType.Desk;

        BaseBuildingOpened();
        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskOpened?.Invoke();
    }

    async void ShowAbilitiesUI(InputAction.CallbackContext ctx)
    {
        if (_openBuilding == DashboardBuildingType.Abilities)
            return;
        if (_openBuilding == DashboardBuildingType.Camp)
            await HideCampUI();
        _openBuilding = DashboardBuildingType.Abilities;

        BaseBuildingOpened();

        _abilitiesWrapperRight.style.left = Length.Percent(100);
        _mainAbilities.style.display = DisplayStyle.Flex;
        DOTween.To(() => _abilitiesWrapperLeft.style.left.value.value,
                x => _abilitiesWrapperLeft.style.left = Length.Percent(x), 0, 0.5f)
                .SetEase(Ease.OutBounce);

        DOTween.To(() => _abilitiesWrapperRight.style.left.value.value,
                x => _abilitiesWrapperRight.style.left = Length.Percent(x), 70, 0.5f)
                .SetEase(Ease.OutBounce);
        await Task.Delay(150);
        float y = -3f;
        for (int i = 0; i < 4; i++)
        {
            EffectHolder instance = Instantiate(PanelOpenEffect);
            instance.PlayEffect(new Vector3(3.5f, y, 1f), Vector3.one * 0.2f);
            y += 2f;
        }

        OnAbilitiesOpened?.Invoke();
    }

    async Task HideAbilityUI()
    {
        DOTween.To(() => _abilitiesWrapperLeft.style.left.value.value,
                x => _abilitiesWrapperLeft.style.left = Length.Percent(x), -70, 0.5f);
        await DOTween.To(() => _abilitiesWrapperRight.style.left.value.value,
                x => _abilitiesWrapperRight.style.left = Length.Percent(x), 100, 0.5f)
                .AsyncWaitForCompletion();
        _mainAbilities.style.display = DisplayStyle.None;
    }

    async void ShowCampUI(InputAction.CallbackContext ctx)
    {
        if (_openBuilding == DashboardBuildingType.Camp)
            return;
        if (_openBuilding == DashboardBuildingType.Abilities)
            await HideAbilityUI();

        _openBuilding = DashboardBuildingType.Camp;
        BaseBuildingOpened();
        _mainCamp.style.display = DisplayStyle.Flex;

        _mainCamp.style.top = Length.Percent(-110);
        DOTween.To(() => _mainCamp.style.top.value.value,
                x => _mainCamp.style.top = Length.Percent(x), 0, 0.5f)
                .SetEase(Ease.OutBounce);
        await Task.Delay(150);

        float x = -6f;
        for (int i = 0; i < 5; i++)
        {
            EffectHolder instance = Instantiate(PanelOpenEffect);
            instance.PlayEffect(new Vector3(x, -5f, 1f), Vector3.one * 0.2f);
            x += 3f;
        }

        OnCampOpened?.Invoke();
    }

    async Task HideCampUI()
    {
        await DOTween.To(() => _mainCamp.style.top.value.value,
                x => _mainCamp.style.top = Length.Percent(x), -110, 0.5f)
                .AsyncWaitForCompletion();
        _mainCamp.style.display = DisplayStyle.None;
    }

    void BaseBuildingOpened()
    {
        HideAllPanels();
        _main.style.display = DisplayStyle.Flex;
    }

    void HideAllPanels(InputAction.CallbackContext ctx) { HideAllPanels(); }
    void HideAllPanels()
    {
        _mainCamp.style.display = DisplayStyle.None;
        _mainAbilities.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
    }


#if UNITY_EDITOR

    [ContextMenu("Add 10000 Gold")]
    void Add100Gold()
    { _gameManager.ChangeGoldValue(10000); }

    [ContextMenu("Remove 5000 Gold")]
    void Remove50Gold()
    { _gameManager.ChangeGoldValue(-5000); }

    [ContextMenu("Add 500 spice")]
    void Add500Spice()
    { _gameManager.ChangeSpiceValue(500); }

    [ContextMenu("Level Up")]
    void LevelUpAllCharacters()
    {
        foreach (Character c in _gameManager.PlayerTroops)
            c.LevelUp();
    }
#endif

}
