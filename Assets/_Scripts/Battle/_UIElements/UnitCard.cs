using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class UnitCard : TooltipCard
    {
        private const string _ussCommonButton = "common__button";

        private const string _ussClassName = "unit-card__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussElement = _ussClassName + "element";

        protected readonly Unit Unit;
        private ResourceBarElement _healthBar;
        private Label _nameLabel;
        private NatureElement _natureElement;

        private UnitIcon _unitIcon;
        protected Label LevelLabel;

        public UnitCard(Unit unit)
        {
            Initialize();

            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitCardStyles);
            if (ss != null) styleSheets.Add(ss);

            Unit = unit;

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

        private void HandleUnitIcon()
        {
            _unitIcon = new(Unit);
            TopLeftContainer.Add(_unitIcon);
        }

        private void HandleNature()
        {
            _natureElement = new(Unit.Nature);
            _natureElement.AddToClassList(_ussElement);
            TopLeftContainer.Add(_natureElement);
        }

        private void HandleNameLabel()
        {
            _nameLabel = new(Unit.UnitName);
            _nameLabel.AddToClassList(USSName);
            TopRightContainer.Add(_nameLabel);
        }

        protected virtual void HandleLevelLabel()
        {
            LevelLabel = new();
            LevelLabel.text = $"Level {Unit.Level.Value}";
            TopRightContainer.Add(LevelLabel);

            Unit.Level.OnValueChanged += i => { LevelLabel.text = $"Level {i}"; };
        }

        private void HandleHealthBar()
        {
            Color c = GameManager.GameDatabase.GetColorByName("Health").Primary;

            _healthBar = new(c, "health", Unit.CurrentHealth, totalStat: Unit.MaxHealth);
            TopRightContainer.Add(_healthBar);
        }

        private void HandleDeath()
        {
            if (Unit.Team != 0) return;
            if (Unit.CurrentHealth.Value > 0) return;

            // HERE: balance price
            PurchaseButton b = new("Revive", _ussCommonButton, Revive, 10);
            BottomContainer.Add(b);

            if (FightManager.IsFightActive)
            {
                b.SetEnabled(false);
                FightManager.Instance.OnFightEnded += () => b.SetEnabled(true);
            }
        }

        private void Revive()
        {
            Unit.Revive();
        }
    }
}