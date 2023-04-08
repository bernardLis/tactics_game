using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroStatsCard : VisualElement
{
    GameManager _gameManager;

    public Hero Hero;

    public HeroPortraitElement PortraitVisualElement;
    StarRankElement _rankElement;
    GoldElement _wageGoldElement;
    ElementalElement _elementalElement;
    Label _title;
    Label _level;

    StatElement _power;
    StatElement _armor;
    StatElement _speed;

    public ResourceBarElement ExpBar;
    public ResourceBarElement HealthBar;
    public ResourceBarElement ManaBar;

    public List<ItemSlot> ItemSlots = new();
    public List<ItemElement> ItemElements = new();

    public List<AbilitySlot> AbilitySlots = new();

    VisualElement _levelUpAnimationContainer;

    bool _showOnlyExp;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextSecondary = "common__text-secondary";


    const string _ussClassName = "hero-stats-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussTopLeftPanel = _ussClassName + "top-left-panel";

    const string _ussBottomPanel = _ussClassName + "bottom-panel";
    const string _ussBottomLeftPanel = _ussClassName + "bottom-left-panel";
    const string _ussBottomRightPanel = _ussClassName + "bottom-right-panel";

    const string _ussElementPosition = _ussClassName + "element-position";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussHealthContainer = _ussClassName + "health-container";
    const string _ussManaContainer = _ussClassName + "mana-container";

    public HeroStatsCard(Hero hero)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroStatsCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        VisualElement topPanel = new();
        topPanel.AddToClassList(_ussTopPanel);
        VisualElement topLeftPanel = new();
        VisualElement topRightPanel = new();
        topPanel.Add(topLeftPanel);
        topPanel.Add(topRightPanel);

        VisualElement bottomPanel = new();
        bottomPanel.AddToClassList(_ussBottomPanel);
        VisualElement bottomLeftPanel = new();
        VisualElement bottomRightPanel = new();
        bottomPanel.Add(bottomLeftPanel);
        bottomPanel.Add(bottomRightPanel);

        PopulateTopLeftPanel(topLeftPanel);
        PopulateTopRightPanel(topRightPanel);
        PopulateBottomLeftPanel(bottomLeftPanel);
        PopulateBottomRightPanel(bottomRightPanel);

        Add(topPanel);
        Add(bottomPanel);

        Hero.OnRankChanged += OnRankChanged;
        Hero.OnElementChanged += OnElementChanged;

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
    }


    void OnRankChanged(HeroRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Hero.HeroName}";
    }

    void OnElementChanged(Element element) { _elementalElement.ChangeElement(element); }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        Hero.OnRankChanged -= OnRankChanged;
        Hero.OnElementChanged -= OnElementChanged;
    }

    void PopulateTopLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopLeftPanel);
        PortraitVisualElement = new(Hero, this);
        container.Add(PortraitVisualElement);
    }

    void PopulateTopRightPanel(VisualElement container)
    {
        VisualElement elementAndRank = new();
        elementAndRank.style.flexDirection = FlexDirection.Row;
        elementAndRank.Add(CreateElementalElement());
        elementAndRank.Add(CreateRankElement());
        container.Add(elementAndRank);

        _title = new($"[{Hero.Rank.Title}] {Hero.HeroName}");
        container.Add(_title);

        container.Add(CreateHealthGroup());
        container.Add(CreateManaGroup());
        container.Add(CreateExpGroup());
    }
    void PopulateBottomLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussBottomLeftPanel);

        GameDatabase db = _gameManager.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), Hero.Power);
        _armor = new(db.GetStatIconByName("Armor"), Hero.Armor);
        _speed = new(db.GetStatIconByName("Speed"), Hero.Speed);

        container.Add(_power);
        container.Add(_armor);
        container.Add(_speed);
    }

    void PopulateBottomRightPanel(VisualElement container)
    {
        container.AddToClassList(_ussBottomRightPanel);
    }

    VisualElement CreateElementalElement()
    {
        _elementalElement = new ElementalElement(Hero.Element);
        _elementalElement.AddToClassList(_ussElementPosition);
        return _elementalElement;
    }

    VisualElement CreateRankElement()
    {
        _rankElement = new(Hero.Rank.Rank, 0.5f);
        return _rankElement;
    }

    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        // TODO: this should be handled differently.
        IntVariable totalExp = ScriptableObject.CreateInstance<IntVariable>();
        totalExp.SetValue(100);
        ExpBar = new(Color.black, "Experience", Hero.Experience, totalExp, null, thickness: 0, isIncreasing: true);

        _level = new Label($"Level {Hero.Level.Value}");
        _level.style.position = Position.Absolute;
        _level.AddToClassList(_ussCommonTextPrimary);
        ExpBar.Add(_level);

        container.Add(ExpBar);
        return container;
    }

    VisualElement CreateHealthGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussHealthContainer);

        // TODO: this should be handled differently.
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue(Hero.Health.GetValue());
        Hero.Health.OnValueChanged += currentHealth.SetValue;

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health", currentHealth, totalValueStat: Hero.Health);
        container.Add(HealthBar);

        return container;
    }

    VisualElement CreateManaGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussManaContainer);

        // TODO: this should be handled differently.
        IntVariable currentMana = ScriptableObject.CreateInstance<IntVariable>();
        currentMana.SetValue(Hero.Mana.GetValue());
        Hero.Mana.OnValueChanged += currentMana.SetValue;

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", currentMana, totalValueStat: Hero.Mana);
        container.Add(ManaBar);

        return container;
    }

}
