using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Upgrades
{
    public class UpgradeScreen : FullScreenElement
    {
        const string _commonOddBackground = "common__odd-background";
        const string _commonEvenBackground = "common__even-background";
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "upgrade-screen__";
        const string _ussMain = _ussClassName + "main";
        const string _ussHeaderContainer = _ussClassName + "header-container";
        const string _ussUpgradeContainer = _ussClassName + "upgrade-container";

        readonly UpgradeBoard _upgradeBoard;

        readonly ScrollView _upgradeContainer;

        bool _isEven;

        public UpgradeScreen(UpgradeBoard upgradeBoard)
        {
            GameManager = GameManager.Instance;
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UpgradeScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            _upgradeBoard = upgradeBoard;

            AddHeader();

            _upgradeContainer = new();
            _upgradeContainer.style.flexDirection = FlexDirection.Row;
            _upgradeContainer.style.flexWrap = Wrap.Wrap;
            Content.Add(_upgradeContainer);

            CreateHeroUpgrades();
            CreateAbilityUpgrades();
            CreateTeamUpgrades();
            CreateCreatureUpgrades();
            CreateBossUpgrades();
            CreateOtherUpgrades();

            AddNavigationButtons();
        }

        void AddHeader()
        {
            VisualElement container = new();
            container.AddToClassList(_ussHeaderContainer);
            Content.Add(container);

            Label title = new("Click & hold to purchase");
            title.style.fontSize = 24;
            container.Add(title);

            GoldElement goldElement = new(GameManager.Gold);
            goldElement.MakeItBig();
            goldElement.style.alignSelf = Align.Center;
            container.Add(goldElement);
            GameManager.OnGoldChanged += goldElement.ChangeAmount;
        }

        void CreateHeroUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Hero Upgrades");
            AddUpgradesByType(container, UpgradeType.Hero);
        }

        void CreateAbilityUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Ability Upgrades");
            AddUpgradesByType(container, UpgradeType.Ability);
        }

        void CreateTeamUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Troops Upgrades");
            AddUpgradesByType(container, UpgradeType.Troops);
        }

        void CreateCreatureUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Creature Upgrades");
            AddUpgradesByType(container, UpgradeType.Creature);
        }

        void CreateBossUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Boss Upgrades");
            AddUpgradesByType(container, UpgradeType.Boss);
        }

        void CreateOtherUpgrades()
        {
            VisualElement container = CreateUpgradeContainer("Other Upgrades");
            AddUpgradesByType(container, UpgradeType.Other);
        }

        VisualElement CreateUpgradeContainer(string txt)
        {
            VisualElement container = new();
            container.AddToClassList(_ussUpgradeContainer);
            _upgradeContainer.Add(container);
            container.AddToClassList(_isEven ? _commonEvenBackground : _commonOddBackground);
            _isEven = !_isEven;

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

        void AddUpgradesByType(VisualElement c, UpgradeType t)
        {
            var upgrades = _upgradeBoard.GetUpgradesByType(t);
            foreach (Upgrade upgrade in upgrades)
            {
                upgrade.Initialize(_upgradeBoard);
                UpgradeElement element = new(upgrade);
                c.Add(element);
            }
        }

        void AddNavigationButtons()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.SpaceAround;
            Add(container);

            // MyButton unlockAllButton = new("Unlock All", _ussCommonButton, _upgradeBoard.UnlockAll);
            // container.Add(unlockAllButton);

            MyButton refundAllButton = new("Refund All", _ussCommonButton, RefundAll);
            container.Add(refundAllButton);

            MyButton backButton = new("Back", _ussCommonButton, Hide);
            container.Add(backButton);

            MyButton playButton = new("Play", _ussCommonButton, Play);
            container.Add(playButton);
        }

        void RefundAll()
        {
            _upgradeBoard.RefundAll();
        }

        void Play()
        {
            Hide();
            OnHide += () => GameManager.LoadScene(Scenes.HeroSelection);
        }
    }
}