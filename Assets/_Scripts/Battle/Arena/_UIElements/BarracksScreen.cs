using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BarracksScreen : FullScreenElement
    {
        Barracks _barracks;

        Label _levelLabel;
        Label _peasantsLabel;
        PurchaseButton _upgradeButton;
        PurchaseButton _buyPeasantButton;

        VisualElement _naturesContainer;

        VisualElement _topContainer;
        VisualElement _bottomContainer;

        public void InitializeBuilding(Barracks b)
        {
            _barracks = b;
            SetTitle("Barracks");
            CreateContainers();

            AddDescription();
            if (_barracks.Level <= _barracks.MaxLevel) AddUpgradeButton();

            AddBuyPeasantButton();
            AddUnlockNaturesButtons();

            AddContinueButton();
        }


        void CreateContainers()
        {
            _topContainer = new();
            _topContainer.style.flexDirection = FlexDirection.Row;
            _topContainer.style.flexGrow = 1;
            Content.Add(_topContainer);

            Content.Add(new HorizontalSpacerElement());

            _bottomContainer = new();
            _bottomContainer.style.flexDirection = FlexDirection.Row;
            _bottomContainer.style.flexGrow = 1;

            Content.Add(_bottomContainer);
        }

        void AddDescription()
        {
            VisualElement container = new();
            _topContainer.Add(container);

            _levelLabel = new($"Level: {_barracks.Level + 1}");
            _peasantsLabel = new($"Spawns {_barracks.GetPeasantsPerFight()} peasants per fight.");
            SetDescriptionText();
            container.Add(_levelLabel);
            container.Add(_peasantsLabel);
        }

        void SetDescriptionText()
        {
            _levelLabel.text = $"Level: {_barracks.Level + 1}";
            _peasantsLabel.text = $"Spawns {_barracks.GetPeasantsPerFight()} peasants per fight.";
        }

        void AddUpgradeButton()
        {
            VisualElement container = new();
            _topContainer.Add(container);

            container.Add(new Label("Upgrade: "));

            _upgradeButton = new("", USSCommonButton, Upgrade, _barracks.GetUpgradePrice());
            container.Add(_upgradeButton);
        }

        void Upgrade()
        {
            if (GameManager.Gold < _barracks.GetUpgradePrice())
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, _upgradeButton, "Not enough gold!", Color.red);
                return;
            }

            _barracks.Upgrade();
            SetDescriptionText();

            if (_barracks.Level <= _barracks.MaxLevel)
            {
                _upgradeButton.ChangePrice(_barracks.GetUpgradePrice());
                return;
            }

            _upgradeButton.SetEnabled(false);
            _upgradeButton.text = "Max level";
            _upgradeButton.RemovePrice();
        }

        void AddBuyPeasantButton()
        {
            VisualElement container = new();
            _topContainer.Add(container);

            container.Add(new Label("Buy Peasant: "));

            _buyPeasantButton = new("", USSCommonButton, BuyPeasant, 100);
            container.Add(_buyPeasantButton);
        }

        void BuyPeasant()
        {
            if (GameManager.Gold < 100)
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, _buyPeasantButton, "Not enough gold!", Color.red);
                return;
            }

            GameManager.ChangeGoldValue(-100);
            Unit u = ScriptableObject.Instantiate(GameManager.UnitDatabase.Peasant);
            u.InitializeBattle(0);
            HeroManager.Instance.Hero.Army.Add(u); // without update to spawn at position
            UnitController uc = FightManager.Instance.SpawnPlayerUnit(u, transform.position);
            uc.GoBackToLocker();
        }

        void AddUnlockNaturesButtons()
        {
            foreach (BarracksNatureUpgrade n in _barracks.UnlockableNatures)
            {
                BarracksNatureUpgradeElement b = new(n);
                _bottomContainer.Add(b);
            }
        }
    }
}