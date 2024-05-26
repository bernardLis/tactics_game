using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BarracksScreen : FullScreenElement
    {
        Barracks _barracks;

        Label _levelLabel;
        PurchaseButton _upgradeButton;

        VisualElement _naturesContainer;

        public void InitializeBuilding(Barracks b)
        {
            _barracks = b;
            SetTitle("Barracks");

            _levelLabel = new($"Level: {_barracks.Level + 1}, spawns {_barracks.Level + 1} peasants per fight.");
            Content.Add(_levelLabel);

            if (_barracks.Level <= _barracks.MaxLevel) AddUpgradeButton();

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
            _levelLabel.text = $"Level: {_barracks.Level + 1}, spawns {_barracks.Level + 1} peasants per fight.";

            if (_barracks.Level <= _barracks.MaxLevel)
            {
                _upgradeButton.ChangePrice(_barracks.Level * 200);
                return;
            }

            _upgradeButton.SetEnabled(false);
            _upgradeButton.text = "Max level";
            _upgradeButton.RemovePrice();
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