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

    public Entity Entity;

    protected VisualElement _container;

    protected VisualElement _nameContainer;
    protected VisualElement _topContainer;
    protected VisualElement _bottomContainer;

    protected VisualElement _topLeftContainer;
    protected VisualElement _topMiddleContainer;
    protected VisualElement _topRightContainer;

    protected VisualElement _bottomLeftContainer;
    protected VisualElement _bottomMiddleContainer;
    protected VisualElement _bottomRightContainer;


    Label _nameLabel;
    protected EntityIcon _entityIcon;

    Label _levelLabel;
    Label _priceLabel;
    Label _maxHealth;

    ElementalElement _element;
    Label _armor;
    Label _speed;

    public EntityCardFull(Entity entity) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Entity = entity;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _container = new();
        _container.AddToClassList(_ussContent);
        _content.Add(_container);

        AddName();
        CreateContainers();
        AddIdentityInfo();
        AddBasicInfo();
        AddBattleCharacteristics();

        AddContinueButton();
    }

    void AddName()
    {
        _nameContainer = new();
        Label n = new(Entity.EntityName);
        n.style.fontSize = 34;
        _nameContainer.Add(n);
        _container.Add(_nameContainer);
    }

    void CreateContainers()
    {
        _topContainer = new();
        _topContainer.style.flexDirection = FlexDirection.Row;
        _bottomContainer = new();
        _bottomContainer.style.flexDirection = FlexDirection.Row;

        _container.Add(_topContainer);
        _container.Add(_bottomContainer);

        _topLeftContainer = new();
        _topMiddleContainer = new();
        _topRightContainer = new();

        _topContainer.Add(_topLeftContainer);
        _topContainer.Add(_topMiddleContainer);
        _topContainer.Add(_topRightContainer);

        _bottomLeftContainer = new();
        _bottomMiddleContainer = new();
        _bottomRightContainer = new();

        _bottomContainer.Add(_bottomLeftContainer);
        _bottomContainer.Add(_bottomMiddleContainer);
        _bottomContainer.Add(_bottomRightContainer);
    }

    void AddIdentityInfo()
    {
        _nameLabel = new($"<b>{Helpers.ParseScriptableObjectName(Entity.name)}<b>");
        _entityIcon = new(Entity, true, true);

        _topLeftContainer.Add(_nameLabel);
        _topLeftContainer.Add(_entityIcon);
    }

    void AddBasicInfo()
    {
        _element = new(Entity.Element);
        _levelLabel = new($"Level: {Entity.Level}");
        _priceLabel = new($"Price: {Entity.Price}");

        _topMiddleContainer.Add(_element);
        _topMiddleContainer.Add(_levelLabel);
        _topMiddleContainer.Add(_priceLabel);

    }

    void AddBattleCharacteristics()
    {
        _maxHealth = new($"Max Health: {Entity.GetMaxHealth()}");
        _armor = new($"Armor: {Entity.Armor}");
        _speed = new($"Speed: {Entity.Speed}");

        _topRightContainer.Add(_maxHealth);
        _topRightContainer.Add(_armor);
        _topRightContainer.Add(_speed);
    }
}
