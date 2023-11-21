using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradesScreen : FullScreenElement
{

    GlobalUpgradeBoard _globalUpgradeBoard;
    public GlobalUpgradesScreen(GlobalUpgradeBoard globalUpgradeBoard)
    {
        _globalUpgradeBoard = globalUpgradeBoard;

        Label title = new("Purchase Global Upgrades");
        _content.Add(title);

        GoldElement goldElement = new(_gameManager.Gold);
        _content.Add(goldElement);
        _gameManager.OnGoldChanged += goldElement.ChangeAmount;

        CreateHeroUpgrades();
        CreateBuildingUpgrades();
        CreateCreatureUpgrades();
        CreateBossUpgrades();
        CreateOtherUpgrades();

        MyButton backButton = new("Back", "common__menu-button", Hide);
        Add(backButton);

        MyButton playButton = new("Play", "common__menu-button", Play);
        Add(playButton);
    }

    void Play() { _gameManager.Play(); }

    void CreateHeroUpgrades()
    {
        VisualElement parentContainer = new();
        _content.Add(parentContainer);
        Label title = new("Hero Upgrades");
        parentContainer.Add(title);

        VisualElement container = new();
        parentContainer.Add(container);
        container.style.flexDirection = FlexDirection.Row;

        container.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroSpeed));
        container.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroArmor));
        container.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroHealth));
        container.Add(new GlobalUpgradeElement(_globalUpgradeBoard.HeroPower));
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

}
