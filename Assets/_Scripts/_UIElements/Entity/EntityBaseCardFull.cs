using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityBaseCardFull : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    protected const string _ussClassName = "entity-card-full__";
    const string _ussContent = _ussClassName + "content";
    const string _ussInfoContainer = _ussClassName + "info-container";
    const string _ussStatsContainer = _ussClassName + "stats-container";
    const string _ussOtherContainer = _ussClassName + "other-container";

    public EntityBase Entity;

    protected ScrollView _mainCardContainer;

    protected VisualElement _nameContainer;
    protected VisualElement _mainContainer;

    protected VisualElement _basicInfoContainer;
    protected VisualElement _statsContainer;
    protected VisualElement _otherContainer;

    protected EntityIcon _entityIcon;

    public EntityBaseCardFull(EntityBase entity) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Entity = entity;

        AddToClassList(_ussCommonTextPrimary);

        _mainCardContainer = new();
        _mainCardContainer.AddToClassList(_ussContent);
        _content.Add(_mainCardContainer);
    }

    public virtual void Initialize()
    {
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

        _basicInfoContainer.AddToClassList(_ussInfoContainer);
        _statsContainer.AddToClassList(_ussStatsContainer);
        _otherContainer.AddToClassList(_ussOtherContainer);

        _mainCardContainer.Add(_basicInfoContainer);

        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        _mainCardContainer.Add(spacer);

        _mainCardContainer.Add(_statsContainer);

        VisualElement spacer1 = new();
        spacer1.AddToClassList(_ussCommonHorizontalSpacer);
        _mainCardContainer.Add(spacer1);

        _mainCardContainer.Add(_otherContainer);
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
        Label l = new($"<b>Level {Entity.Level.Value} {Helpers.ParseScriptableObjectName(Entity.name)}<b>");
        _basicInfoContainer.Add(l);
    }

    void AddElement()
    {
        ElementalElement e = new(Entity.Element);
        _basicInfoContainer.Add(e);
    }

    protected virtual void AddStats()
    {
        StatElement maxHealth = new(Entity.MaxHealth);
        StatElement armor = new(Entity.Armor);

        _statsContainer.Add(maxHealth);
        _statsContainer.Add(armor);
    }

    protected virtual void AddOtherBasicInfo()
    {
        Label price = new($"Price: {Entity.Price}");
        _otherContainer.Add(price);
    }

}
