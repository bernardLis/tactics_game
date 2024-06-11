using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Pawn;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BarracksNatureUpgradeElement : VisualElement
    {
        private const string _ussCommonButton = "common__button";
        private const string _ussClassName = "barracks-nature-upgrade-element__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussPawnLocked = _ussClassName + "pawn-locked";
        private const string _ussPawnUnlocked = _ussClassName + "pawn-unlocked";

        private readonly BarracksNatureUpgrade _barracksNatureUpgrade;

        private readonly GameManager _gameManager;

        private readonly Label _levelLabel;
        private readonly List<UnitIcon> _pawnIcons = new();
        private readonly PurchaseButton _purchaseButton;

        public BarracksNatureUpgradeElement(BarracksNatureUpgrade n)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.BarracksNatureUpgradeElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _barracksNatureUpgrade = n;

            _levelLabel = new($"Level: {n.CurrentLevel}");
            Add(_levelLabel);

            AddPawnElements();

            Add(new NatureElement(n.Nature));
            Add(new Label($"{n.Description}"));

            UpdatePawnIconStyles();

            if (n.IsMaxLevel()) return;
            _purchaseButton = new("", _ussCommonButton, Unlock, n.GetUpgradePrice());
            Add(_purchaseButton);
        }

        private void AddPawnElements()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            Add(container);

            Pawn p = _gameManager.UnitDatabase.GetPawnByNature(_barracksNatureUpgrade.Nature);
            for (int i = 0; i < p.Upgrades.Length; i++)
            {
                Pawn newPawn = Object.Instantiate(p);
                newPawn.InitializeBattle(1);
                for (int j = 0; j < i; j++)
                    newPawn.Upgrade();
                UnitIcon pawnIcon = new(newPawn);
                container.Add(pawnIcon);
                _pawnIcons.Add(pawnIcon);
            }
        }

        private void Unlock()
        {
            _barracksNatureUpgrade.Upgrade();
            UpdateLevelLabel();
            UpdatePawnIconStyles();
            UpdatePurchaseButton();
        }

        private void UpdateLevelLabel()
        {
            _levelLabel.text = $"Level: {_barracksNatureUpgrade.CurrentLevel}";
        }

        private void UpdatePawnIconStyles()
        {
            for (int i = 0; i < _pawnIcons.Count; i++)
                if (i < _barracksNatureUpgrade.CurrentLevel)
                {
                    _pawnIcons[i].RemoveFromClassList(_ussPawnLocked);
                    _pawnIcons[i].AddToClassList(_ussPawnUnlocked);
                }
                else
                {
                    _pawnIcons[i].RemoveFromClassList(_ussPawnUnlocked);
                    _pawnIcons[i].AddToClassList(_ussPawnLocked);
                }
        }

        public void UpdatePurchaseButton()
        {
            if (_barracksNatureUpgrade.IsMaxLevel())
                _purchaseButton.RemoveFromHierarchy();
            else
                _purchaseButton.ChangePrice(_barracksNatureUpgrade.GetUpgradePrice());
        }
    }
}