using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityCardFull : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "entity-card-full__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";

    GameManager _gameManager;

    public Entity Entity;

    VisualElement _content;
    VisualElement _leftContainer;
    VisualElement _middleContainer;
    VisualElement _rightContainer;

    Label _nameLabel;
    EntityIcon _entityIcon;

    Label _levelLabel;
    Label _priceLabel;
    Label _maxHealth;

    ElementalElement _element;
    Label _armor;
    Label _speed;

    ContinueButton _continueButton;

    public EntityCardFull(Entity entity)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize();

        Entity = entity;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        _leftContainer = new();
        _middleContainer = new();
        _rightContainer = new();

        _content.Add(_leftContainer);
        _content.Add(_middleContainer);
        _content.Add(_rightContainer);

        AddIdentityInfo();
        AddBasicStats();
        AddBattleStats();

        _continueButton = new("Continue", callback: Hide);
        Add(_continueButton);
    }

    void AddIdentityInfo()
    {
        _nameLabel = new($"<b>{Helpers.ParseScriptableObjectName(Entity.name)}<b>");
        _entityIcon = new(Entity, true, true);

        _leftContainer.Add(_nameLabel);
        _leftContainer.Add(_entityIcon);
    }

    void AddBasicStats()
    {
        _element = new(Entity.Element);
        _levelLabel = new($"Level: {Entity.Level}");
        _maxHealth = new($"Max Health: {Entity.GetMaxHealth()}");

        _middleContainer.Add(_element);
        _middleContainer.Add(_levelLabel);
        _middleContainer.Add(_maxHealth);
    }

    void AddBattleStats()
    {
        _armor = new($"Armor: {Entity.Armor}");
        _speed = new($"Speed: {Entity.Speed}");
        _priceLabel = new($"Price: {Entity.Price}");

        _rightContainer.Add(_armor);
        _rightContainer.Add(_speed);
        _rightContainer.Add(_priceLabel);
    }
}
