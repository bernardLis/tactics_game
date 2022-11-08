using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DashboardManager : MonoBehaviour
{
    GameManager _gameManager;

    public VisualElement Root { get; private set; }

    // buttons
    VisualElement _navDesk;
    Label _navDay;

    VisualElement _navArmory;
    VisualElement _navAbilities;
    VisualElement _navShop;

    // resources
    VisualElement _navGold;
    GoldElement _goldElement;

    VisualElement _mainArmory;
    VisualElement _mainAbilities;
    VisualElement _mainShop;
    VisualElement _mainDesk;

    VisualElement _activeNavTab;

    public event Action OnDeskClicked;
    public event Action OnArmoryClicked;
    public event Action OnAbilitiesClicked;
    public event Action OnShopClicked;
    public event Action OnHideAllPanels;

    void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.OnDayPassed += UpdateDay;

        Root = GetComponent<UIDocument>().rootVisualElement;
        _navDesk = Root.Q<VisualElement>("navDesk");
        _navDay = Root.Q<Label>("navDay");

        _navArmory = Root.Q<VisualElement>("navArmory");
        _navAbilities = Root.Q<VisualElement>("navAbilities");
        _navShop = Root.Q<VisualElement>("navShop");

        // resources
        _navGold = Root.Q<VisualElement>("navGold");

        _navDesk.RegisterCallback<PointerUpEvent>(NavDeskClick);
        _navArmory.RegisterCallback<PointerUpEvent>(NavArmoryClick);
        _navAbilities.RegisterCallback<PointerUpEvent>(NavAbilitiesClick);
        _navShop.RegisterCallback<PointerUpEvent>(NavShopClick);

        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _mainArmory = Root.Q<VisualElement>("mainArmory");
        _mainAbilities = Root.Q<VisualElement>("mainAbilities");
        _mainShop = Root.Q<VisualElement>("mainShop");

        UpdateDay(_gameManager.Day);
        AddGoldElement();
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

        _mainAbilities.style.display = DisplayStyle.Flex;
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

    void NavClick(PointerUpEvent e)
    {
        VisualElement target = (VisualElement)e.currentTarget;
        HideAllPanels();
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
        _mainAbilities.style.display = DisplayStyle.None;
        _mainShop.style.display = DisplayStyle.None;
        _mainDesk.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
    }

    void UpdateDay(int day) { _navDay.text = $"Day: {day}"; }

    void AddGoldElement()
    {
        _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += _goldElement.ChangeAmount;
        _navGold.Add(_goldElement);
    }

#if UNITY_EDITOR

    [ContextMenu("Add 100 Gold")]
    void Add100Gold()
    {
        _gameManager.ChangeGoldValue(100);
    }

    [ContextMenu("Remove 50 Gold")]
    void Remove50Gold()
    {
        _gameManager.ChangeGoldValue(-50);
    }


#endif

}
