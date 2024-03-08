using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class UnitCard : TooltipCard
    {
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

            var ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitCardStyles);
            if (ss != null) styleSheets.Add(ss);

            _unit = unit;

            AddToClassList(_ussMain);
            PopulateCard();
        }

        protected void PopulateCard()
        {
            HandleUnitIcon();
            HandleElementalElement();
            HandleNameLabel();
            HandleLevelLabel();
            HandleHealthBar();
        }

        void HandleUnitIcon()
        {
            _unitIcon = new(_unit);
            _topLeftContainer.Add(_unitIcon);
        }

        protected virtual void HandleElementalElement()
        {
            _natureElement = new(_unit.Nature);
            _natureElement.AddToClassList(_ussElement);
            _topLeftContainer.Add(_natureElement);
        }

        protected virtual void HandleNameLabel()
        {
            _nameLabel = new(_unit.UnitName);
            _nameLabel.AddToClassList(_ussName);
            _topRightContainer.Add(_nameLabel);
        }

        void HandleLevelLabel()
        {
            _levelLabel = new();
            _levelLabel.text = $"Level {_unit.Level.Value}";
            _topRightContainer.Add(_levelLabel);

            _unit.Level.OnValueChanged += (i) => { _levelLabel.text = $"Level {i}"; };
        }

        void HandleHealthBar()
        {
            Color c = _gameManager.GameDatabase.GetColorByName("Health").Primary;

            _healthBar = new(c, "health", currentFloatVar: _unit.CurrentHealth, totalStat: _unit.MaxHealth);
            _topRightContainer.Add(_healthBar);
        }
    }
}