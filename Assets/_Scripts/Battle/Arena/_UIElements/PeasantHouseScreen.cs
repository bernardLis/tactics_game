using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class PeasantHouseScreen : FullScreenElement
    {
        Building _building;

        Label _levelLabel;
        PurchaseButton _upgradeButton;

        public void InitializeBuilding(Building building)
        {
            _building = building;

            Label title = new Label("Peasant House");
            Content.Add(title);

            _levelLabel = new Label($"Level: {_building.Level + 1}, spawns {_building.Level + 1} peasants per fight.");
            Content.Add(_levelLabel);

            if (_building.Level <= _building.MaxLevel) AddUpgradeButton();

            AddContinueButton();
        }

        void AddUpgradeButton()
        {
            _upgradeButton = new("", USSCommonButton, Upgrade, (_building.Level + 1) * 200);
            Content.Add(_upgradeButton);
        }

        void Upgrade()
        {
            if (GameManager.Gold < _building.Level * 200)
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, _upgradeButton, "Not enough gold!", Color.red);
                return;
            }

            _building.Upgrade();
            _levelLabel.text = $"Level: {_building.Level + 1}, spawns {_building.Level + 1} peasants per fight.";

            if (_building.Level <= _building.MaxLevel)
            {
                _upgradeButton.ChangePrice(_building.Level * 200);
                return;
            }

            _upgradeButton.SetEnabled(false);
            _upgradeButton.text = "Max level";
            _upgradeButton.RemovePrice();
        }
    }
}