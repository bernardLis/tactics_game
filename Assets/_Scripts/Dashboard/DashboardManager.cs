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
    VisualElement _navQuests;
    VisualElement _navArmory;
    VisualElement _navAbilities;
    VisualElement _navShop;
    VisualElement _navPassDay;

    // resources
    VisualElement _navDay;
    VisualElement _navGold;
    GoldElement _goldElement;

    VisualElement _mainQuests;
    VisualElement _mainArmory;
    VisualElement _mainAbilities;
    VisualElement _mainShop;

    VisualElement _activeNavTab;

    public event Action OnQuestsClicked;
    public event Action OnArmoryClicked;
    public event Action OnAbilitiesClicked;
    public event Action OnShopClicked;

    void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.OnDayPassed += UpdateDay;

        Root = GetComponent<UIDocument>().rootVisualElement;
        _navQuests = Root.Q<VisualElement>("navQuests");
        _navArmory = Root.Q<VisualElement>("navArmory");
        _navAbilities = Root.Q<VisualElement>("navAbilities");
        _navShop = Root.Q<VisualElement>("navShop");
        _navPassDay = Root.Q<VisualElement>("navPassDay");

        // resources
        _navDay = Root.Q<VisualElement>("navDay");
        _navGold = Root.Q<VisualElement>("navGold");

        _navQuests.RegisterCallback<PointerUpEvent>(NavQuestsClick);
        _navArmory.RegisterCallback<PointerUpEvent>(NavArmoryClick);
        _navAbilities.RegisterCallback<PointerUpEvent>(NavAbilitiesClick);
        _navShop.RegisterCallback<PointerUpEvent>(NavShopClick);
        _navPassDay.RegisterCallback<PointerUpEvent>(NavPassDay);

        _mainQuests = Root.Q<VisualElement>("mainQuests");
        _mainArmory = Root.Q<VisualElement>("mainArmory");
        _mainAbilities = Root.Q<VisualElement>("mainAbilities");
        _mainShop = Root.Q<VisualElement>("mainShop");

        UpdateDay(_gameManager.Day);
        AddGoldElement();
    }

    void NavQuestsClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navQuests)
            return;

        NavClick(e);

        _mainQuests.style.display = DisplayStyle.Flex;
        OnQuestsClicked?.Invoke();

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

    void NavPassDay(PointerUpEvent e)
    {
        HideAllPanels();
        if (_activeNavTab != null)
        {
            _activeNavTab.RemoveFromClassList("navTabActive");
            _activeNavTab.AddToClassList("navTab");
        }
        _activeNavTab = null;

        _gameManager.PassDay();
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
        _mainQuests.style.display = DisplayStyle.None;
        _mainArmory.style.display = DisplayStyle.None;
        _mainAbilities.style.display = DisplayStyle.None;
        _mainShop.style.display = DisplayStyle.None;
    }

    void UpdateDay(int dayNumber)
    {
        _navDay.Clear();

        Label day = new Label($"Day: {dayNumber}");
        _navDay.Add(day);
    }

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
