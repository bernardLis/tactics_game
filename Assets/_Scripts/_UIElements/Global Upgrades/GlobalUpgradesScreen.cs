using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradesScreen : FullScreenElement
{
    const string _ussClassName = "global-upgrade-screen__";
    const string _ussMain = _ussClassName + "main";
    const string _ussHeaderContainer = _ussClassName + "header-container";
    const string _ussUpgradeContainer = _ussClassName + "upgrade-container";

    bool _isGray;

    GlobalUpgradeBoard _globalUpgradeBoard;

    ScrollView _upgradeContainer;

    public GlobalUpgradesScreen(GlobalUpgradeBoard globalUpgradeBoard)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GlobalUpgradeScreenStyles);
        if (ss != null) styleSheets.Add(ss);

        _globalUpgradeBoard = globalUpgradeBoard;
        style.backgroundColor = new Color(0, 0, 0, 1f);

        AddHeader();

        _upgradeContainer = new();
        _upgradeContainer.style.flexDirection = FlexDirection.Row;
        _upgradeContainer.style.flexWrap = Wrap.Wrap;
        _content.Add(_upgradeContainer);

        CreateHeroUpgrades();
        CreateAbilityUpgrades();
        CreateBuildingUpgrades();
        CreateCreatureUpgrades();
        CreateBossUpgrades();
        CreateOtherUpgrades();

        AddNavigationButtons();
    }

    void AddHeader()
    {
        VisualElement container = new();
        container.AddToClassList(_ussHeaderContainer);
        _content.Add(container);

        Label title = new("Click & hold to purchase");
        title.style.fontSize = 24;
        container.Add(title);

        GoldElement goldElement = new(_gameManager.Gold);
        goldElement.MakeItBig();
        goldElement.style.alignSelf = Align.Center;
        container.Add(goldElement);
        _gameManager.OnGoldChanged += goldElement.ChangeAmount;

    }

    void CreateHeroUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Hero Upgrades");

        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Hero);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeElement element = new(upgrade);
            container.Add(element);
        }
    }

    void CreateAbilityUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Ability Upgrades");

        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Ability);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeElement element = new(upgrade);
            container.Add(element);
        }
    }

    void CreateBuildingUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Building Upgrades");

        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Building);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeBuildingElement element = new(upgrade);
            container.Add(element);
        }
    }


    void CreateCreatureUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Creature Upgrades");

        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Creature);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeElement element = new(upgrade);
            container.Add(element);
        }
    }

    void CreateBossUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Boss Upgrades");
        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Boss);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeElement element = new(upgrade);
            container.Add(element);
        }
    }


    void CreateOtherUpgrades()
    {
        VisualElement container = CreateUpgradeContainer("Other Upgrades");

        List<GlobalUpgrade> upgrades = _globalUpgradeBoard.GetGlobalUpgradesByType(GlobalUpgradeType.Other);
        foreach (GlobalUpgrade upgrade in upgrades)
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeElement element = new(upgrade);
            container.Add(element);
        }
    }


    VisualElement CreateUpgradeContainer(string txt)
    {
        VisualElement container = new();
        container.AddToClassList(_ussUpgradeContainer);
        _upgradeContainer.Add(container);
        Color c = new Color(0.3f, 0.3f, 0.3f);
        if (_isGray) c = new Color(0.6f, 0.6f, 0.6f);
        container.style.backgroundColor = c;

        _isGray = !_isGray;

        Label title = new(txt);
        title.style.width = 300;
        title.style.fontSize = 24;
        title.style.whiteSpace = WhiteSpace.Normal;
        container.Add(title);

        VisualElement upgradeContainer = new();
        upgradeContainer.style.flexDirection = FlexDirection.Row;
        upgradeContainer.style.flexWrap = Wrap.Wrap;
        container.Add(upgradeContainer);

        return upgradeContainer;

    }

    void AddNavigationButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceAround;
        Add(container);

        MyButton refundAllButton = new("Refund All", "common__menu-button", RefundAll);
        container.Add(refundAllButton);

        MyButton backButton = new("Back", "common__menu-button", Hide);
        container.Add(backButton);

        MyButton playButton = new("Play", "common__menu-button", Play);
        container.Add(playButton);
    }

    void RefundAll()
    {
        _globalUpgradeBoard.RefundAll();
    }

    void Play()
    {
        Hide();
        OnHide += () => _gameManager.StartGame();
        // schedule.Execute(() => _gameManager.Play()).StartingIn(1000);

    }

}
