using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DashboardManager : Singleton<DashboardManager>
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "dashboard__";
    const string _ussPassDayButton = _ussClassName + "pass-day-button";
    const string _ussNavIcon = _ussClassName + "nav-icon";
    const string _ussNavDesk = _ussClassName + "nav-desk";
    const string _ussNavCamp = _ussClassName + "nav-camp";
    const string _ussNavAbilities = _ussClassName + "nav-abilities";
    const string _ussNavArchive = _ussClassName + "nav-archive";
    const string _ussNavMenu = _ussClassName + "nav-menu";
    const string _ussTimerWrapper = _ussClassName + "timer-wrapper";
    const string _ussTimerLine = _ussClassName + "timer-line";

    GameManager _gameManager;
    public PlayerInput PlayerInput { get; private set; }

    public VisualElement Root { get; private set; }

    public EffectHolder PanelOpenEffect;

    public LineTimerElement DayTimer;

    // resources
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

    [SerializeField] Sound _dashboardTheme;

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

        AddDayTimer();

        // resources
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
        AddSpiceElement();

        AddNavigationButtons();

        // AudioManager.Instance.PlayMusic(_dashboardTheme);
    }

    /* INPUT */
    void SubscribeInputActions()
    {
        PlayerInput.actions["Pause"].performed += TogglePause;
        PlayerInput.actions["OpenDesk"].performed += ShowDeskUI;
        PlayerInput.actions["OpenCamp"].performed += ShowCampUI;
        PlayerInput.actions["OpenAbilities"].performed += ShowAbilityUI;

        PlayerInput.actions["CloseCurrentTab"].performed += HideAllPanels;
    }

    void UnsubscribeInputActions()
    {
        PlayerInput.actions["Pause"].performed -= TogglePause;
        PlayerInput.actions["OpenDesk"].performed -= ShowDeskUI;
        PlayerInput.actions["OpenCamp"].performed -= ShowCampUI;
        PlayerInput.actions["OpenAbilities"].performed -= ShowAbilityUI;

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

    void TogglePause(InputAction.CallbackContext ctx) { _gameManager.ToggleTimer(!_gameManager.IsTimerOn); }
    void ShowDeskUI(InputAction.CallbackContext ctx) { StartCoroutine(ShowDeskUICoroutine()); }
    void ShowAbilityUI(InputAction.CallbackContext ctx) { StartCoroutine(ShowAbilityUICoroutine()); }
    void ShowCampUI(InputAction.CallbackContext ctx) { StartCoroutine(ShowCampUICoroutine()); }

    void AddDayTimer()
    {
        DayTimer = new(_gameManager.SecondsLeftInDay, GameManager.SecondsInDay, true, $"Day: {_gameManager.Day}");
        DayTimer.SetStyles(_ussTimerWrapper, _ussTimerLine);
        DayTimer.OnLoopFinished += _gameManager.PassDay;
        Root.Q<VisualElement>("navLeft").Add(DayTimer);
    }

    /* RESOURCES */
    void UpdateDay(int day) { DayTimer.UpdateLabel($"Day: {day}"); }

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
        int troopsCount = _gameManager.GetAllCharacters().Count;
        int troopsLimit = _troopsLimitBuilding.GetUpgradeByRank(_troopsLimitBuilding.UpgradeRank).TroopsLimit;

        _troopsLimitElement.UpdateCountContainer($"{troopsCount} / {troopsLimit}", Color.white);

    }

    void AddSpiceElement()
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
        MyButton navMenu = new(null, _ussNavMenu, OnMenuClick);

        navDesk.AddToClassList(_ussNavIcon);
        navCamp.AddToClassList(_ussNavIcon);
        navAbilities.AddToClassList(_ussNavIcon);
        navArchive.AddToClassList(_ussNavIcon);
        navMenu.AddToClassList(_ussNavIcon);

        navRight.Add(navDesk);
        navRight.Add(navCamp);
        navRight.Add(navAbilities);
        navRight.Add(navArchive);
        navRight.Add(navMenu);
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

    void OnMenuClick()
    {
        _gameManager.GetComponent<GameUIManager>().ToggleMenu(new InputAction.CallbackContext());
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
            ShowAbilityUI(a);
    }


    IEnumerator ShowDeskUICoroutine()
    {
        if (_openBuilding == DashboardBuildingType.Desk)
            yield break;
        if (_openBuilding == DashboardBuildingType.Camp)
            yield return HideCampUI();
        if (_openBuilding == DashboardBuildingType.Abilities)
            yield return HideAbilityUI();

        _openBuilding = DashboardBuildingType.Desk;

        BaseBuildingOpened();
        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskOpened?.Invoke();
    }

    IEnumerator ShowAbilityUICoroutine()
    {
        if (_openBuilding == DashboardBuildingType.Abilities)
            yield break;
        if (_openBuilding == DashboardBuildingType.Camp)
            yield return HideCampUI();

        _openBuilding = DashboardBuildingType.Abilities;

        BaseBuildingOpened();

        _mainAbilities.style.display = DisplayStyle.Flex;
        if (!_gameManager.HideMenuEffects)
        {
            yield return PlayAbilityMenuOpenTransition();
        }
        else
        {
            _abilitiesWrapperLeft.style.left = Length.Percent(0);
            _abilitiesWrapperRight.style.left = Length.Percent(70);
        }

        OnAbilitiesOpened?.Invoke();
    }

    IEnumerator PlayAbilityMenuOpenTransition()
    {
        _abilitiesWrapperRight.style.left = Length.Percent(100);
        DOTween.To(() => _abilitiesWrapperLeft.style.left.value.value,
                x => _abilitiesWrapperLeft.style.left = Length.Percent(x), 0, 0.5f)
                .SetEase(Ease.OutBounce);

        DOTween.To(() => _abilitiesWrapperRight.style.left.value.value,
                x => _abilitiesWrapperRight.style.left = Length.Percent(x), 70, 0.5f)
                .SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(0.15f);

        float y = -3f;
        for (int i = 0; i < 4; i++)
        {
            EffectHolder instance = Instantiate(PanelOpenEffect);
            instance.PlayEffect(new Vector3(3.5f, y, 1f), Vector3.one * 0.2f);
            y += 2f;
        }
    }

    IEnumerator ShowCampUICoroutine()
    {
        if (_openBuilding == DashboardBuildingType.Camp)
            yield break;
        if (_openBuilding == DashboardBuildingType.Abilities)
            yield return HideAbilityUI();

        _openBuilding = DashboardBuildingType.Camp;
        BaseBuildingOpened();
        _mainCamp.style.display = DisplayStyle.Flex;

        if (!_gameManager.HideMenuEffects)
            yield return PlayCampMenuOpenTransition();
        else
            _mainCamp.style.top = Length.Percent(0);

        OnCampOpened?.Invoke();
    }

    IEnumerator PlayCampMenuOpenTransition()
    {
        _mainCamp.style.top = Length.Percent(-110);
        DOTween.To(() => _mainCamp.style.top.value.value,
                x => _mainCamp.style.top = Length.Percent(x), 0, 0.5f)
                .SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(0.15f);

        float x = -6f;
        for (int i = 0; i < 5; i++)
        {
            EffectHolder instance = Instantiate(PanelOpenEffect);
            instance.PlayEffect(new Vector3(x, -5f, 1f), Vector3.one * 0.2f);
            x += 3f;
        }
    }

    IEnumerator HideAbilityUI()
    {
        if (!_gameManager.HideMenuEffects)
        {
            DOTween.To(() => _abilitiesWrapperLeft.style.left.value.value,
                    x => _abilitiesWrapperLeft.style.left = Length.Percent(x), -70, 0.5f);
            yield return DOTween.To(() => _abilitiesWrapperRight.style.left.value.value,
                    x => _abilitiesWrapperRight.style.left = Length.Percent(x), 100, 0.5f)
                    .WaitForCompletion();
        }

        _mainAbilities.style.display = DisplayStyle.None;
    }

    IEnumerator HideCampUI()
    {
        if (!_gameManager.HideMenuEffects)
        {
            yield return DOTween.To(() => _mainCamp.style.top.value.value,
                    x => _mainCamp.style.top = Length.Percent(x), -110, 0.5f)
                    .WaitForCompletion();
        }
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
