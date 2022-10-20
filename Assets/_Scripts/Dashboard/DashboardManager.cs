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
    VisualElement _navDaySummary;
    Label _dayNumberLabel;

    VisualElement _navQuests;
    VisualElement _navArmory;
    VisualElement _navAbilities;
    VisualElement _navShop;

    // resources
    VisualElement _navGold;
    GoldElement _goldElement;

    VisualElement _mainQuests;
    VisualElement _mainArmory;
    VisualElement _mainAbilities;
    VisualElement _mainShop;
    VisualElement _mainDaySummary;

    VisualElement _activeNavTab;

    public event Action OnDaySummaryClicked;
    public event Action OnQuestsClicked;
    public event Action OnArmoryClicked;
    public event Action OnAbilitiesClicked;
    public event Action OnShopClicked;
    public event Action OnHideAllPanels;

    void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.OnDayPassed += UpdateDay;

        Root = GetComponent<UIDocument>().rootVisualElement;
        _navDaySummary = Root.Q<VisualElement>("navDaySummary");
        _dayNumberLabel = Root.Q<Label>("dayNumberLabel");

        _navQuests = Root.Q<VisualElement>("navQuests");
        _navArmory = Root.Q<VisualElement>("navArmory");
        _navAbilities = Root.Q<VisualElement>("navAbilities");
        _navShop = Root.Q<VisualElement>("navShop");

        // resources
        _navGold = Root.Q<VisualElement>("navGold");

        _navDaySummary.RegisterCallback<PointerUpEvent>(NavDaySummaryClick);
        _navQuests.RegisterCallback<PointerUpEvent>(NavQuestsClick);
        _navArmory.RegisterCallback<PointerUpEvent>(NavArmoryClick);
        _navAbilities.RegisterCallback<PointerUpEvent>(NavAbilitiesClick);
        _navShop.RegisterCallback<PointerUpEvent>(NavShopClick);

        _mainDaySummary = Root.Q<VisualElement>("mainDaySummary");
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

    void NavDaySummaryClick(PointerUpEvent e)
    {
        if (_activeNavTab == _navDaySummary)
            return;

        NavClick(e);

        _mainDaySummary.style.display = DisplayStyle.Flex;
        OnDaySummaryClicked?.Invoke();
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
        _mainDaySummary.style.display = DisplayStyle.None;

        OnHideAllPanels?.Invoke();
    }

    void UpdateDay(int dayNumber)
    {
        _dayNumberLabel.text = $"Day: {dayNumber}";
        HideAllPanels();
        _mainDaySummary.style.display = DisplayStyle.Flex;
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
