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

        /*
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

                _globalUpgradeBoard.HeroSprint.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement sprint = new(_globalUpgradeBoard.HeroSprint);
                container.Add(sprint);

                _globalUpgradeBoard.HeroGrab.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement grab = new(_globalUpgradeBoard.HeroGrab);
                container.Add(grab);
                */
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

        /*
                _globalUpgradeBoard.BuildingUpgrades.ForEach(upgrade =>
                {
                    upgrade.Initialize(_globalUpgradeBoard);
                    GlobalUpgradeBuildingElement element = new(upgrade);
                    container.Add(element);
                });
                */
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

        /*
                _globalUpgradeBoard.CreatureArmor.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement ca = new(_globalUpgradeBoard.CreatureArmor);
                container.Add(ca);

                _globalUpgradeBoard.CreatureHealth.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement ch = new(_globalUpgradeBoard.CreatureHealth);
                container.Add(ch);

                _globalUpgradeBoard.CreaturePower.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement cp = new(_globalUpgradeBoard.CreaturePower);
                container.Add(cp);

                _globalUpgradeBoard.CreatureSpeed.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement cs = new(_globalUpgradeBoard.CreatureSpeed);
                container.Add(cs);

                _globalUpgradeBoard.CreatureStartingLevel.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement csl = new(_globalUpgradeBoard.CreatureStartingLevel);
                container.Add(csl);
                */
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

        /*
                _globalUpgradeBoard.BossCorruptionBreakNodes.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement bcbn = new(_globalUpgradeBoard.BossCorruptionBreakNodes);
                container.Add(bcbn);

                _globalUpgradeBoard.BossCorruptionDuration.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement bcd = new(_globalUpgradeBoard.BossCorruptionDuration);
                container.Add(bcd);

                _globalUpgradeBoard.BossSlowdown.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement bs = new(_globalUpgradeBoard.BossSlowdown);
                container.Add(bs);

                _globalUpgradeBoard.BossStunDuration.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement bsd = new(_globalUpgradeBoard.BossStunDuration);
                container.Add(bsd);
                */
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

        /*
                _globalUpgradeBoard.RewardNumber.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement rn = new(_globalUpgradeBoard.RewardNumber);
                container.Add(rn);

                _globalUpgradeBoard.RewardReroll.Initialize(_globalUpgradeBoard);
                GlobalUpgradeElement rr = new(_globalUpgradeBoard.RewardReroll);
                container.Add(rr);
                */
    }


    VisualElement CreateUpgradeContainer(string txt)
    {
        VisualElement container = new();
        container.AddToClassList(_ussUpgradeContainer);
        _upgradeContainer.Add(container);
        // Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Color c = new Color(0.3f, 0.3f, 0.3f);
        if (_isGray) c = new Color(0.6f, 0.6f, 0.6f);
        container.style.backgroundColor = c;
        // container.style.borderBottomColor = randomColor;
        // container.style.borderTopColor = randomColor;

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
