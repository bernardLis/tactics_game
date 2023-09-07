using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityBaseCardFull : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    protected const string _ussClassName = "entity-card-full__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";

    public EntityBase Entity;

    protected ScrollView _mainCardContainer;

    protected VisualElement _nameContainer;
    protected VisualElement _mainContainer;

    protected VisualElement _basicInfoContainer;
    protected VisualElement _statsContainer;
    protected VisualElement _otherContainer;

    Label _nameLabel;
    protected EntityIcon _entityIcon;

    Label _levelLabel;
    Label _priceLabel;
    ElementalElement _element;

    StatElement _maxHealth;
    StatElement _armor;

    public EntityBaseCardFull(EntityBase entity) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Entity = entity;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _mainCardContainer = new();
        _mainCardContainer.AddToClassList(_ussContent);
        _content.Add(_mainCardContainer);

        CreateContainers();
        AddName();
        AddIcon();
        AddLevel();
        AddElement();
        AddOtherBasicInfo();
        AddStats();

        AddContinueButton();
    }

    void CreateContainers()
    {
        _basicInfoContainer = new();
        _statsContainer = new();
        _otherContainer = new();

        Add(_basicInfoContainer);
        Add(_statsContainer);
        Add(_otherContainer);
    }

    void AddName()
    {
        _nameContainer = new();
        Label n = new(Entity.EntityName);
        n.style.fontSize = 34;
        _nameContainer.Add(n);
        _basicInfoContainer.Add(_nameContainer);
    }

    void AddIcon()
    {
        _entityIcon = new(Entity, true, true);
        _basicInfoContainer.Add(_entityIcon);
    }

    void AddLevel()
    {
        Label l = new($"Level: {Entity.Level.Value} <b>{Helpers.ParseScriptableObjectName(Entity.name)}<b>");
        _basicInfoContainer.Add(l);
    }

    void AddElement()
    {
        ElementalElement e = new(Entity.Element);
        _basicInfoContainer.Add(e);
    }

    protected virtual void AddOtherBasicInfo()
    {
        _priceLabel = new($"Price: {Entity.Price}");
        _basicInfoContainer.Add(_priceLabel);
    }

    protected virtual void AddStats()
    {
        _maxHealth = new(Entity.MaxHealth);
        _armor = new(Entity.Armor);

        _statsContainer.Add(_maxHealth);
        _statsContainer.Add(_armor);
    }
}
