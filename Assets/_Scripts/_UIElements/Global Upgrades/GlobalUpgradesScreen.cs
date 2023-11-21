using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradesScreen : FullScreenElement
{
    GlobalUpgradeBoard _globalUpgradeBoard;

    VisualElement _upgradeContainer;

    public GlobalUpgradesScreen(GlobalUpgradeBoard globalUpgradeBoard)
    {
        _globalUpgradeBoard = globalUpgradeBoard;

        AddHeader();

        _upgradeContainer = new();
        _upgradeContainer.style.flexDirection = FlexDirection.Row;
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
        _upgradeContainer.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroSpeed));
        _upgradeContainer.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroArmor));
        _upgradeContainer.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroHealth));
        _upgradeContainer.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroPower));
    }

    void CreateBuildingUpgrades()
    {

    }


    void CreateCreatureUpgrades()
    {

    }

    void CreateBossUpgrades()
    {

    }


    void CreateOtherUpgrades()
    {

    }

    void AddNavigationButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceAround;
        Add(container);

        MyButton backButton = new("Back", "common__menu-button", Hide);
        container.Add(backButton);

        MyButton playButton = new("Play", "common__menu-button", Play);
        container.Add(playButton);
    }

    void Play() { _gameManager.Play(); }

}
