using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class UnitCard : TooltipCard
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "unit-card__";
        const string _ussMain = _ussClassName + "main";
        const string _ussElement = _ussClassName + "element";

        UnitIcon _unitIcon;
        NatureElement _natureElement;
        Label _nameLabel;
        Label _levelLabel;
        ResourceBarElement _healthBar;

        readonly Unit _unit;

        public UnitCard(Unit unit)
        {
            Initialize();

            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitCardStyles);
            if (ss != null) styleSheets.Add(ss);

            _unit = unit;

            AddToClassList(_ussMain);
            PopulateCard();
        }

        protected void PopulateCard()
        {
            TopLeftContainer.Clear();
            TopRightContainer.Clear();

            HandleUnitIcon();
            HandleNature();
            HandleNameLabel();
            HandleLevelLabel();
            HandleHealthBar();
            HandleDeath();
        }

        void HandleUnitIcon()
        {
            _unitIcon = new(_unit);
            TopLeftContainer.Add(_unitIcon);
        }

        void HandleNature()
        {
            _natureElement = new(_unit.Nature);
            _natureElement.AddToClassList(_ussElement);
            TopLeftContainer.Add(_natureElement);
        }

        void HandleNameLabel()
        {
            _nameLabel = new(_unit.UnitName);
            _nameLabel.AddToClassList(USSName);
            TopRightContainer.Add(_nameLabel);
        }

        void HandleLevelLabel()
        {
            _levelLabel = new();
            _levelLabel.text = $"Level {_unit.Level.Value}";
            TopRightContainer.Add(_levelLabel);

            _unit.Level.OnValueChanged += (i) => { _levelLabel.text = $"Level {i}"; };
        }

        void HandleHealthBar()
        {
            Color c = GameManager.GameDatabase.GetColorByName("Health").Primary;

            _healthBar = new(c, "health", currentFloatVar: _unit.CurrentHealth, totalStat: _unit.MaxHealth);
            TopRightContainer.Add(_healthBar);
        }

        void HandleDeath()
        {
            if (_unit.Team != 0) return;
            if (_unit.CurrentHealth.Value > 0) return;

            // HERE: balance price
            PurchaseButton b = new("Revive", _ussCommonButton, Revive, 100);
            BottomContainer.Add(b);

            if (FightManager.IsFightActive)
            {
                b.SetEnabled(false);
                FightManager.Instance.OnFightEnded += () => b.SetEnabled(true);
            }

        }

        void Revive()
        {
            _unit.Revive();
        }
    }
}