using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class UnitCard : TooltipCard
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "unit-card__";
        const string _ussMain = _ussClassName + "main";
        const string _ussElement = _ussClassName + "element";

        protected readonly Unit Unit;
        ResourceBarElement _healthBar;
        Label _nameLabel;
        NatureElement _natureElement;

        UnitIcon _unitIcon;
        protected Label LevelLabel;

        PurchaseButton _reviveButton;

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

        void HandleUnitIcon()
        {
            _unitIcon = new(Unit);
            TopLeftContainer.Add(_unitIcon);
        }

        void HandleNature()
        {
            _natureElement = new(Unit.Nature);
            _natureElement.AddToClassList(_ussElement);
            TopLeftContainer.Add(_natureElement);
        }

        void HandleNameLabel()
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

        void HandleHealthBar()
        {
            Color c = GameManager.GameDatabase.GetColorByName("Health").Primary;

            _healthBar = new(c, "health", Unit.CurrentHealth, totalStat: Unit.MaxHealth);
            TopRightContainer.Add(_healthBar);
        }

        void HandleDeath()
        {
            if (Unit.Team != 0) return;
            if (Unit.CurrentHealth.Value > 0) return;

            // HERE: balance price
            _reviveButton = new("Revive", _ussCommonButton, Revive, 10);
            BottomContainer.Add(_reviveButton);

            if (FightManager.IsFightActive)
            {
                _reviveButton.SetEnabled(false);
                FightManager.Instance.OnFightEnded += () => _reviveButton.SetEnabled(true);
            }
        }

        void Revive()
        {
            Unit.Revive();
            _reviveButton.RemoveFromHierarchy();
        }
    }
}