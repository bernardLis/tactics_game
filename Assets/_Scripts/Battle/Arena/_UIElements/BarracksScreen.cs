using System.Collections.Generic;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BarracksScreen : FullScreenElement
    {
        readonly List<BarracksNatureUpgradeElement> _natureElements = new();
        Barracks _barracks;
        BarracksController _barracksController;
        VisualElement _bottomContainer;
        PurchaseButton _buyPeasantButton;

        Label _levelLabel;

        VisualElement _naturesContainer;
        Label _peasantsLabel;

        VisualElement _topContainer;


        public void InitializeBuilding(Barracks b, BarracksController bc)
        {
            _barracks = b;
            _barracksController = bc;

            SetTitle("Barracks");
            CreateContainers();

            AddDescription();

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
            _barracksController.SpawnPeasant();
        }

        void AddUnlockNaturesButtons()
        {
            foreach (BarracksNatureUpgrade n in _barracks.UnlockableNatures)
            {
                n.OnUpgrade += (_, _) => UpdateUnlockNaturesButtons();
                BarracksNatureUpgradeElement b = new(n);
                _natureElements.Add(b);
                _bottomContainer.Add(b);
            }
        }

        void UpdateUnlockNaturesButtons()
        {
            _barracks.DisableUpgradeToken();
            foreach (BarracksNatureUpgradeElement b in _natureElements)
                b.UpdatePurchaseButton();
        }
    }
}