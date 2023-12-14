using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityCard : TooltipCard
{
    const string _ussClassName = "entity-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussElement = _ussClassName + "element";

    public EntityIcon EntityIcon;
    protected ElementalElement _elementalElement;
    protected Label _nameLabel;
    protected Label _levelLabel;
    protected ResourceBarElement _healthBar;

    public Entity Entity;
    public EntityCard(Entity entity)
    {
        Initialize();

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardStyles);
        if (ss != null) styleSheets.Add(ss);

        Entity = entity;

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
        EntityIcon = new(Entity);
        _topLeftContainer.Add(EntityIcon);
    }

    protected virtual void HandleElementalElement()
    {
        _elementalElement = new(Entity.Element);
        _elementalElement.AddToClassList(_ussElement);
        _topLeftContainer.Add(_elementalElement);
    }

    protected virtual void HandleNameLabel()
    {
        _nameLabel = new(Entity.EntityName);
        _nameLabel.AddToClassList(_ussName);
        _topRightContainer.Add(_nameLabel);
    }


    protected virtual void HandleLevelLabel()
    {
        _levelLabel = new();
        _levelLabel.text = $"Level {Entity.Level.Value}";
        _topRightContainer.Add(_levelLabel);

        Entity.Level.OnValueChanged += (i) =>
        {
            _levelLabel.text = $"Level {i}";
        };
    }

    protected virtual void HandleHealthBar()
    {
        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;

        _healthBar = new(c, "health", currentIntVar: Entity.CurrentHealth, totalStat: Entity.MaxHealth);
        _topRightContainer.Add(_healthBar);
    }
}
