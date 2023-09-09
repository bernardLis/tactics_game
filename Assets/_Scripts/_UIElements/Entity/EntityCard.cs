using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "entity-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLeftContainer = _ussClassName + "left-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussRightContainer = _ussClassName + "right-container";
    const string _ussName = _ussClassName + "name";
    const string _ussElement = _ussClassName + "element";

    protected GameManager _gameManager;

    protected VisualElement _leftContainer;
    protected VisualElement _middleContainer;
    protected VisualElement _rightContainer;

    public EntityIcon EntityIcon;
    protected ElementalElement _elementalElement;
    protected Label _nameLabel;
    protected ResourceBarElement _expBar;
    protected Label _levelLabel;
    protected ResourceBarElement _healthBar;

    public Entity Entity;
    public EntityCard(Entity entity)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Entity = entity;

        AddToClassList(_ussMain);

        _leftContainer = new();
        _leftContainer.AddToClassList(_ussLeftContainer);
        _middleContainer = new();
        _middleContainer.AddToClassList(_ussMiddleContainer);
        _rightContainer = new();
        _rightContainer.AddToClassList(_ussRightContainer);

        Add(_leftContainer);
        Add(_middleContainer);
        Add(_rightContainer);
    }

    protected virtual void PopulateCard()
    {
        HandleEntityIcon();
        HandleElementalElement();
        HandleNameLabel();
        HandleExpBar();
        HandleLevelLabel();
        HandleHealthBar();
    }

    protected virtual void HandleEntityIcon()
    {
        EntityIcon = new(Entity, true);
        _leftContainer.Add(EntityIcon);
    }

    protected virtual void HandleElementalElement()
    {
        _elementalElement = new(Entity.Element);
        _elementalElement.AddToClassList(_ussElement);
        _leftContainer.Add(_elementalElement);
    }

    protected virtual void HandleNameLabel()
    {
        _nameLabel = new(Entity.EntityName);
        _nameLabel.AddToClassList(_ussName);
        _middleContainer.Add(_nameLabel);
    }

    protected virtual void HandleExpBar()
    {
        _expBar = new(_gameManager.GameDatabase.GetColorByName("Experience").Color,
                "Experience", Entity.Experience, Entity.ExpForNextLevel);
        _middleContainer.Add(_expBar);
    }

    protected virtual void HandleLevelLabel()
    {
        _levelLabel = new();
        _levelLabel.text = $"Level {Entity.Level.Value}";
        _levelLabel.style.position = Position.Absolute;
        _levelLabel.style.left = 5;
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _expBar.Add(_levelLabel);

        // HERE: something weeird with level
        Entity.Level.OnValueChanged += (i) =>
        {
            _levelLabel.text = $"Level {i}";
        };
    }

    protected virtual void HandleHealthBar()
    {
        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;
        _healthBar = new(c, "health", currentIntVar: Entity.CurrentHealth, totalStat: Entity.MaxHealth);
        _middleContainer.Add(_healthBar);
    }
}
