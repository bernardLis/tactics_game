using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class BarracksScreen : FullScreenElement
    {
        readonly List<BarracksNatureUpgradeElement> _natureElements = new();
        Barracks _barracks;
        BarracksController _barracksController;
        VisualElement _bottomContainer;
        PurchaseButton _buyPeasantButton;


        VisualElement _naturesContainer;
        VisualElement _perFightArmyContainer;

        VisualElement _topContainer;


        public void InitializeBuilding(Barracks b, BarracksController bc)
        {
            _barracks = b;
            _barracksController = bc;

            SetTitle("Barracks");
            CreateContainers();

            AddPerFightArmy();

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

            _perFightArmyContainer = new();
            _topContainer.Add(_perFightArmyContainer);

            Content.Add(new HorizontalSpacerElement());

            _bottomContainer = new();
            _bottomContainer.style.flexDirection = FlexDirection.Row;
            _bottomContainer.style.flexGrow = 1;

            Content.Add(_bottomContainer);
        }

        void AddPerFightArmy()
        {
            _perFightArmyContainer.Clear();

            Label descriptionLabel = new("Every fight you get: ");
            _perFightArmyContainer.Add(descriptionLabel);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            _perFightArmyContainer.Add(container);
            foreach (Unit u in _barracks.GetUnitsPerFight())
            {
                UnitIcon icon = new(u);
                container.Add(icon);
            }
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
        }

        void AddUnlockNaturesButtons()
        {
            foreach (BarracksNatureUpgrade n in _barracks.UnlockableNatures)
            {
                n.OnUpgrade += (_, _) => OnNatureUpgraded();
                BarracksNatureUpgradeElement b = new(n);
                _natureElements.Add(b);
                _bottomContainer.Add(b);
            }
        }

        void OnNatureUpgraded()
        {
            _barracks.DisableUpgradeToken();
            foreach (BarracksNatureUpgradeElement b in _natureElements)
                b.UpdatePurchaseButton();

            AddPerFightArmy();
        }
    }
}