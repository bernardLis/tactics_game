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
    const string _ussNavAbilities = _ussClassName + "nav-abilities";
    const string _ussNavArchive = _ussClassName + "nav-archive";
    const string _ussNavMenu = _ussClassName + "nav-menu";
    const string _ussTimerWrapper = _ussClassName + "timer-wrapper";
    const string _ussTimerLine = _ussClassName + "timer-line";

    GameManager _gameManager;
    public PlayerInput PlayerInput { get; private set; }

    DraggableArmies _draggableArmies;

    public VisualElement Root { get; private set; }

    public EffectHolder PanelOpenEffect;

    public LineTimerElement DayTimer;

    // nav
    VisualElement _navGold;
    GoldElement _goldElement;
    VisualElement _navTroops;
    VisualElement _navSpice;

    TroopsLimitElement _troopsLimitElement;

    VisualElement _main;
    VisualElement _mainAbilities;
    VisualElement _abilitiesWrapperLeft;
    VisualElement _abilitiesWrapperRight;

    DashboardBuildingType _openBuilding = DashboardBuildingType.Desk;

    [SerializeField] Sound _dashboardTheme;

    public event Action OnAbilitiesOpened;
    public event Action OnHideAllPanels;
    protected override void Awake()
    {
        base.Awake();
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += UpdateDay;

        _draggableArmies = GetComponent<DraggableArmies>();

        Root = GetComponent<UIDocument>().rootVisualElement;

        AddDayTimer();

        // resources
        _navGold = Root.Q<VisualElement>("navGold");
        _navTroops = Root.Q<VisualElement>("navTroops");
        _navSpice = Root.Q<VisualElement>("navSpice");

        _main = Root.Q<VisualElement>("main");
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
    }

    /* INPUT */
    void SubscribeInputActions()
    {
        PlayerInput.actions["Pause"].performed += TogglePause;
        PlayerInput.actions["CloseCurrentTab"].performed += HideAllPanels;
    }

    void UnsubscribeInputActions()
    {
        PlayerInput.actions["Pause"].performed -= TogglePause;
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
    void ShowAbilityUI(InputAction.CallbackContext ctx) { StartCoroutine(ShowAbilityUICoroutine()); }

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
        _troopsLimitElement = new($"");
        UpdateTroopsElement();
        _gameManager.OnHeroAddedToTroops += UpdateTroopsElement;

        _navTroops.Add(_troopsLimitElement);
    }

    // I know it is not nice, but it is also nice, and I don't care about the parameters passed in this case
    void UpdateTroopsElement(Hero c) { UpdateTroopsElement(); }
    void UpdateTroopsElement(int i) { UpdateTroopsElement(); }
    void UpdateTroopsElement()
    {
        int troopsCount = _gameManager.GetAllHeroes().Count;
        int troopsLimit = 5;

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
        MyButton navAbilities = new(null, _ussNavAbilities, OpenAbilities);
        MyButton navMenu = new(null, _ussNavMenu, OnMenuClick);

        navAbilities.AddToClassList(_ussNavIcon);
        navMenu.AddToClassList(_ussNavIcon);

        navRight.Add(navAbilities);
        navRight.Add(navMenu);
    }

    void OpenAbilities() { OpenDashboardBuilding(DashboardBuildingType.Abilities); }


    void OnMenuClick()
    {
        _gameManager.GetComponent<GameUIManager>().ToggleMenu(new InputAction.CallbackContext());
    }

    public void OpenDashboardBuilding(DashboardBuildingType db)
    {
        // new InputAction.CallbackContext() - coz it is hooked up in editor to open ui with f keys
        InputAction.CallbackContext a = new InputAction.CallbackContext();

        if (db == DashboardBuildingType.Abilities)
            ShowAbilityUI(a);
    }

    IEnumerator ShowAbilityUICoroutine()
    {
        if (_openBuilding == DashboardBuildingType.Abilities)
            yield break;

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


    void BaseBuildingOpened()
    {
        HideAllPanels();
        _main.style.display = DisplayStyle.Flex;
    }

    void HideAllPanels(InputAction.CallbackContext ctx) { HideAllPanels(); }

    void HideAllPanels()
    {
        _mainAbilities.style.display = DisplayStyle.None;
        OnHideAllPanels?.Invoke();
    }
}
