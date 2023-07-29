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
        _nameLabel = new(Entity.Name);
        _nameLabel.AddToClassList(_ussName);
        _middleContainer.Add(_nameLabel);
    }

    protected virtual void HandleExpBar()
    {
        IntVariable exp = ScriptableObject.CreateInstance<IntVariable>();
        exp.SetValue(0);
        IntVariable expForNextLevel = ScriptableObject.CreateInstance<IntVariable>();
        expForNextLevel.SetValue(100);

        _expBar = new(Color.gray, "Experience", exp, expForNextLevel);
        _middleContainer.Add(_expBar);
    }

    protected virtual void HandleLevelLabel()
    {
        _levelLabel = new Label($"Level {Entity.Level}");
        _levelLabel.style.position = Position.Absolute;
        _levelLabel.style.left = 5;
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _expBar.Add(_levelLabel);
    }

    protected virtual void HandleHealthBar()
    {
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue(Entity.BaseHealth);
        IntVariable totalHealth = ScriptableObject.CreateInstance<IntVariable>();
        totalHealth.SetValue(Entity.BaseHealth);

        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;
        _healthBar = new(c, "health", currentHealth, totalHealth);
        _middleContainer.Add(_healthBar);
    }
}
