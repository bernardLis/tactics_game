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
    public PlayerInput PlayerInput { get; private set; }

    public VisualElement Root { get; private set; }

    public EffectHolder PanelOpenEffect;

    MyButton _passDayButton;

    // resources
    Label _navDay;
    VisualElement _navGold;
    GoldElement _goldElement;
    VisualElement _navTroops;
    VisualElement _navSpice;

    CampBuildingTroopsLimit _troopsLimitBuilding;
    TroopsLimitElement _troopsLimitElement;

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
    }

    void Start()
    {
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
        PlayerInput.actions["OpenDesk"].performed += ShowDeskUI;
        PlayerInput.actions["OpenCamp"].performed += ShowCampUI;
        PlayerInput.actions["OpenAbilities"].performed += ShowAbilitiesUI;

        PlayerInput.actions["CloseCurrentTab"].performed += HideAllPanels;
    }

    void UnsubscribeInputActions()
    {
        PlayerInput.actions["OpenDesk"].performed -= ShowDeskUI;
        PlayerInput.actions["OpenCamp"].performed -= ShowCampUI;
        PlayerInput.actions["OpenAbilities"].performed -= ShowAbilitiesUI;

        PlayerInput.actions["CloseCurrentTab"].performed -= HideAllPanels;
    }

    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
            
        PlayerInput = _gameManager.GetComponent<PlayerInput>();
        PlayerInput.SwitchCurrentActionMap("Dashboard");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (PlayerInput == null)
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
        _troopsLimitBuilding = _gameManager.GetComponent<BuildingManager>().TroopsLimitBuilding;
        _troopsLimitElement = new($"");
        UpdateTroopsElement();
        _gameManager.OnCharacterAddedToTroops += UpdateTroopsElement;
        _troopsLimitBuilding.OnUpgraded += UpdateTroopsElement;

        _navTroops.Add(_troopsLimitElement);
    }

    // I know it is not nice, but it is also nice, and I don't care about the parameters passed in this case
    void UpdateTroopsElement(Character c) { UpdateTroopsElement(); }
    void UpdateTroopsElement(int i) { UpdateTroopsElement(); }
    void UpdateTroopsElement()
    {
        int troopsCount = _gameManager.PlayerTroops.Count;
        int troopsLimit = _troopsLimitBuilding.GetUpgradeByRank(_troopsLimitBuilding.UpgradeRank).TroopsLimit;

        _troopsLimitElement.UpdateCountContainer($"{troopsCount} / {troopsLimit}", Color.white);

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

        _mainAbilities.style.display = DisplayStyle.Flex;
        if (!_gameManager.HideMenuEffects)
        {
            await PlayAbilityMenuOpenTransition();
        }
        else
        {
            _abilitiesWrapperLeft.style.left = Length.Percent(0);
            _abilitiesWrapperRight.style.left = Length.Percent(70);
        }

        OnAbilitiesOpened?.Invoke();
    }

    async Task PlayAbilityMenuOpenTransition()
    {
        _abilitiesWrapperRight.style.left = Length.Percent(100);
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
    }

    async Task HideAbilityUI()
    {
        if (!_gameManager.HideMenuEffects)
        {
            DOTween.To(() => _abilitiesWrapperLeft.style.left.value.value,
                    x => _abilitiesWrapperLeft.style.left = Length.Percent(x), -70, 0.5f);
            await DOTween.To(() => _abilitiesWrapperRight.style.left.value.value,
                    x => _abilitiesWrapperRight.style.left = Length.Percent(x), 100, 0.5f)
                    .AsyncWaitForCompletion();
        }

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

        if (!_gameManager.HideMenuEffects)
            await PlayCampMenuOpenTransition();
        else
            _mainCamp.style.top = Length.Percent(0);

        OnCampOpened?.Invoke();
    }

    async Task PlayCampMenuOpenTransition()
    {
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
    }

    async Task HideCampUI()
    {
        if (!_gameManager.HideMenuEffects)
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
}
