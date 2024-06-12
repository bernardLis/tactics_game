using System.Collections.Generic;
using Lis.Battle.Arena.Building;
using Lis.Core;
using Lis.Units;
using Lis.Units.Pawn;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BarracksNatureUpgradeElement : VisualElement
    {
        const string _ussCommonButton = "common__button";
        const string _ussClassName = "barracks-nature-upgrade-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussPawnLocked = _ussClassName + "pawn-locked";
        const string _ussPawnUnlocked = _ussClassName + "pawn-unlocked";

        readonly BarracksNatureUpgrade _barracksNatureUpgrade;

        readonly GameManager _gameManager;

        readonly Label _levelLabel;
        readonly List<UnitIcon> _pawnIcons = new();
        readonly PurchaseButton _purchaseButton;

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

        void AddPawnElements()
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

        void Unlock()
        {
            _barracksNatureUpgrade.Upgrade();
            UpdateLevelLabel();
            UpdatePawnIconStyles();
            UpdatePurchaseButton();
        }

        void UpdateLevelLabel()
        {
            _levelLabel.text = $"Level: {_barracksNatureUpgrade.CurrentLevel}";
        }

        void UpdatePawnIconStyles()
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