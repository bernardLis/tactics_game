using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradesScreen : FullScreenElement
{
    GlobalUpgradeBoard _globalUpgradeBoard;

    ScrollView _upgradeContainer;

    public GlobalUpgradesScreen(GlobalUpgradeBoard globalUpgradeBoard)
    {
        _globalUpgradeBoard = globalUpgradeBoard;
        style.backgroundColor = new Color(0, 0, 0, 1f);

        AddHeader();

        _upgradeContainer = new();
        _upgradeContainer.style.flexDirection = FlexDirection.Row;
        _upgradeContainer.style.flexWrap = Wrap.Wrap;
        _content.Add(_upgradeContainer);

        CreateHeroUpgrades();
        CreateBuildingUpgrades();
        CreateCreatureUpgrades();
        CreateBossUpgrades();
        CreateOtherUpgrades();

        AddNavigationButtons();
    }

    void AddHeader()
    {
        VisualElement container = new();
        container.style.justifyContent = Justify.Center;
        _content.Add(container);

        GoldElement goldElement = new(_gameManager.Gold);
        goldElement.style.alignSelf = Align.Center;
        container.Add(goldElement);
        _gameManager.OnGoldChanged += goldElement.ChangeAmount;

        Label title = new("Click & hold to purchase Global Upgrades");
        title.style.fontSize = 24;
        container.Add(title);
    }

    void CreateHeroUpgrades()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        _upgradeContainer.Add(container);

        _globalUpgradeBoard.HeroSpeed.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement hs = new(_globalUpgradeBoard.HeroSpeed);
        container.Add(hs);

        _globalUpgradeBoard.HeroArmor.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement ha = new(_globalUpgradeBoard.HeroArmor);
        container.Add(ha);

        _globalUpgradeBoard.HeroHealth.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement hh = new(_globalUpgradeBoard.HeroHealth);
        container.Add(hh);

        _globalUpgradeBoard.HeroPower.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement hp = new(_globalUpgradeBoard.HeroPower);
        container.Add(hp);

        _globalUpgradeBoard.HeroPull.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement hpu = new(_globalUpgradeBoard.HeroPull);
        container.Add(hpu);
    }

    void CreateBuildingUpgrades()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        _upgradeContainer.Add(container);

        _globalUpgradeBoard.BuildingUpgrades.ForEach(upgrade =>
        {
            upgrade.Initialize(_globalUpgradeBoard);
            GlobalUpgradeBuildingElement element = new(upgrade);
            container.Add(element);
        });
    }


    void CreateCreatureUpgrades()
    {

    }

    void CreateBossUpgrades()
    {

    }


    void CreateOtherUpgrades()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        _upgradeContainer.Add(container);

        _globalUpgradeBoard.RewardNumber.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement rn = new(_globalUpgradeBoard.RewardNumber);
        container.Add(rn);

        _globalUpgradeBoard.RewardReroll.Initialize(_globalUpgradeBoard);
        GlobalUpgradeElement rr = new(_globalUpgradeBoard.RewardReroll);
        container.Add(rr);
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
