using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DashboardManager : MonoBehaviour
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

    // buttons
    VisualElement _navArmory;
    VisualElement _navAbilities;
    VisualElement _navShop;
    VisualElement _navDesk;
    VisualElement _navCamp;

    VisualElement _main;
    VisualElement _mainArmory;
    VisualElement _mainShop;
    VisualElement _mainDesk;
    VisualElement _mainCamp;

    VisualElement _activeNavTab;

    public event Action OnDeskClicked;
    public event Action OnArmoryClicked;
    public event Action OnAbilitiesClicked;
    public event Action OnShopClicked;
    public event Action OnBaseClicked;
    public event Action OnHideAllPanels;

    void Awake()
    {
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

        _navDesk = Root.Q<VisualElement>("navDesk");
        _navArmory = Root.Q<VisualElement>("navArmory");
        _navAbilities = Root.Q<VisualElement>("navAbilities");
        _navShop = Root.Q<VisualElement>("navShop");
        _navCamp = Root.Q<VisualElement>("navCamp");

        _navDesk.RegisterCallback<PointerUpEvent>(NavDeskClick);
        _navArmory.RegisterCallback<PointerUpEvent>(NavArmoryClick);
        _navAbilities.RegisterCallback<PointerUpEvent>(NavAbilitiesClick);
        _navShop.RegisterCallback<PointerUpEvent>(NavShopClick);
        _navCamp.RegisterCallback<PointerUpEvent>(NavCampClick);

        _main = Root.Q<VisualElement>("main");
        _mainArmory = Root.Q<VisualElement>("mainArmory");
        _mainShop = Root.Q<VisualElement>("mainShop");
        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _mainCamp = Root.Q<VisualElement>("mainCamp");

        UpdateDay(_gameManager.Day);
        AddGoldElement();
        AddTroopsElement();
        AddSpiceElements();
    }

    void NavArmoryClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navArmory)
            return;

        NavClick(e);

        _mainArmory.style.display = DisplayStyle.Flex;
        OnArmoryClicked?.Invoke();
    }

    void NavAbilitiesClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navAbilities)
            return;

        NavClick(e);
        _main.style.display = DisplayStyle.None;

        OnAbilitiesClicked?.Invoke();
    }

    void NavShopClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navShop)
            return;

        NavClick(e);

        _mainShop.style.display = DisplayStyle.Flex;
        OnShopClicked?.Invoke();
    }

    void NavDeskClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navDesk)
            return;

        NavClick(e);

        _mainDesk.style.display = DisplayStyle.Flex;
        OnDeskClicked?.Invoke();
    }

    void NavCampClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navCamp)
            return;
        NavClick(e);
        _mainCamp.style.display = DisplayStyle.Flex;
        OnBaseClicked?.Invoke();
    }

    void NavClick(PointerUpEvent e)
    {
        VisualElement target = (VisualElement)e.currentTarget;
        HideAllPanels();

        _main.style.display = DisplayStyle.Flex;

        if (_activeNavTab != null)
        {
            _activeNavTab.RemoveFromClassList("navTabActive");
            _activeNavTab.AddToClassList("navTab");
        }

        _activeNavTab = target;
        target.AddToClassList("navTabActive");
        target.RemoveFromClassList("navTab");
    }

    void HideAllPanels()
    {
        _mainArmory.style.display = DisplayStyle.None;
        _mainShop.style.display = DisplayStyle.None;
        _mainDesk.style.display = DisplayStyle.None;
        _mainCamp.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
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
