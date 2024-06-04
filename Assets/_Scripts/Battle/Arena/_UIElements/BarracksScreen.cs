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
        PurchaseButton _upgradeButton;
        PurchaseButton _buyPeasantButton;

        VisualElement _naturesContainer;

        public void InitializeBuilding(Barracks b)
        {
            _barracks = b;
            SetTitle("Barracks");

            _levelLabel = new($"Level: {_barracks.Level + 1}, spawns {_barracks.Level + 1} peasants per fight.");
            Content.Add(_levelLabel);

            if (_barracks.Level <= _barracks.MaxLevel) AddUpgradeButton();

            AddBuyPeasantButton();
            AddUnlockNaturesButtons();

            AddContinueButton();
        }

        void AddUpgradeButton()
        {
            _upgradeButton = new("", USSCommonButton, Upgrade, (_barracks.Level + 1) * 200);
            Content.Add(_upgradeButton);
        }

        void Upgrade()
        {
            if (GameManager.Gold < _barracks.Level * 200)
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, _upgradeButton, "Not enough gold!", Color.red);
                return;
            }

            _barracks.Upgrade();
            _levelLabel.text = $"Level: {_barracks.Level + 2}, spawns {_barracks.Level + 2} peasants per fight.";

            if (_barracks.Level <= _barracks.MaxLevel)
            {
                _upgradeButton.ChangePrice((_barracks.Level + 1) * 200);
                return;
            }

            _upgradeButton.SetEnabled(false);
            _upgradeButton.text = "Max level";
            _upgradeButton.RemovePrice();
        }

        void AddBuyPeasantButton()
        {
            _buyPeasantButton = new("Buy Peasant", USSCommonButton, BuyPeasant, 100);
            Content.Add(_buyPeasantButton);
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
            Content.Add(new Label("Unlock to upgrade Peasants: "));
            _naturesContainer = new();
            _naturesContainer.style.flexDirection = FlexDirection.Row;
            Content.Add(_naturesContainer);

            foreach (UnlockableNature n in _barracks.UnlockableNatures)
            {
                UnlockableNatureElement b = new(n);
                _naturesContainer.Add(b);
            }
        }
    }
}