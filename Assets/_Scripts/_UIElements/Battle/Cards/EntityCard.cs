using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class EntityCard : TooltipCard
    {
        const string _ussClassName = "entity-card__";
        const string _ussMain = _ussClassName + "main";
        const string _ussElement = _ussClassName + "element";

        EntityIcon _entityIcon;
        ElementalElement _elementalElement;
        Label _nameLabel;
        Label _levelLabel;
        ResourceBarElement _healthBar;

        readonly Entity _entity;

        public EntityCard(Entity entity)
        {
            Initialize();

            var ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.EntityCardStyles);
            if (ss != null) styleSheets.Add(ss);

            _entity = entity;

            AddToClassList(_ussMain);
            PopulateCard();
        }

        protected virtual void PopulateCard()
        {
            HandleEntityIcon();
            HandleElementalElement();
            HandleNameLabel();
            HandleLevelLabel();
            HandleHealthBar();
        }

        protected virtual void HandleEntityIcon()
        {
            _entityIcon = new(_entity);
            _topLeftContainer.Add(_entityIcon);
        }

        protected virtual void HandleElementalElement()
        {
            _elementalElement = new(_entity.Element);
            _elementalElement.AddToClassList(_ussElement);
            _topLeftContainer.Add(_elementalElement);
        }

        protected virtual void HandleNameLabel()
        {
            _nameLabel = new(_entity.EntityName);
            _nameLabel.AddToClassList(_ussName);
            _topRightContainer.Add(_nameLabel);
        }

        void HandleLevelLabel()
        {
            _levelLabel = new();
            _levelLabel.text = $"Level {_entity.Level.Value}";
            _topRightContainer.Add(_levelLabel);

            _entity.Level.OnValueChanged += (i) => { _levelLabel.text = $"Level {i}"; };
        }

        void HandleHealthBar()
        {
            Color c = _gameManager.GameDatabase.GetColorByName("Health").Primary;

            _healthBar = new(c, "health", currentFloatVar: _entity.CurrentHealth, totalStat: _entity.MaxHealth);
            _topRightContainer.Add(_healthBar);
        }
    }
}