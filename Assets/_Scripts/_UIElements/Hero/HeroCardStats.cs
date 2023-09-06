using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCardStats : VisualElement
{

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "hero-stats-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussTopLeftPanel = _ussClassName + "top-left-panel";
    const string _ussTopMiddlePanel = _ussClassName + "top-middle-panel";
    const string _ussTopRightPanel = _ussClassName + "top-right-panel";

    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussManaContainer = _ussClassName + "mana-container";

    GameManager _gameManager;

    public Hero Hero;

    HeroCardMini _portrait;
    ElementalElement _elementalElement;
    Label _title;
    Label _level;

    StatElement _power;
    StatElement _armor;
    StatElement _speed;

    public ResourceBarElement ExpBar;
    public ResourceBarElement ManaBar;


    public HeroCardStats(Hero hero)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardStatsStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;
        hero.OnLevelUp += OnLevelUp;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        VisualElement topPanel = new();
        topPanel.AddToClassList(_ussTopPanel);
        VisualElement topLeftPanel = new();
        VisualElement topMiddlePanel = new();
        VisualElement topRightPanel = new();
        topPanel.Add(topLeftPanel);
        // //HERE: disable hero stats
        // topPanel.Add(topMiddlePanel);
        topPanel.Add(topRightPanel);

        PopulateTopLeftPanel(topLeftPanel);
        PopulateTopMiddlePanel(topMiddlePanel);
        PopulateTopRightPanel(topRightPanel);

        Add(topPanel);

        Hero.OnRankChanged += OnRankChanged;

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
    }

    void OnRankChanged(HeroRank rank)
    {
        _title.text = $"[{rank.Title}] {Hero.EntityName}";
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        Hero.OnRankChanged -= OnRankChanged;
    }

    void PopulateTopLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopLeftPanel);
        _portrait = new(Hero);
        _portrait.BlockTooltip();
        container.Add(_portrait);
    }

    public void BlockClick()
    {
        _portrait.BlockClick();
    }

    void PopulateTopMiddlePanel(VisualElement container)
    {
        container.AddToClassList(_ussTopMiddlePanel);

        HeroDatabase db = _gameManager.HeroDatabase;
        // _power = new(db.GetStatIconByName("Power"), Hero.Power);
        _armor = new(db.GetStatIconByName("Armor"), Hero.Armor);
        _speed = new(db.GetStatIconByName("Speed"), Hero.Speed);

        container.Add(_power);
        container.Add(_armor);
        container.Add(_speed);
    }

    void PopulateTopRightPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopRightPanel);

        _title = new($"[{Hero.Rank.Title}] {Hero.EntityName}");
        container.Add(_title);

        container.Add(CreateExpGroup());
        container.Add(CreateManaGroup());
    }

    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        ExpBar = new(Color.gray, "Experience", currentIntVar: Hero.Experience, totalIntVar: Hero.ExpForNextLevel);

        _level = new Label($"Level {Hero.Level.Value}");
        _level.style.position = Position.Absolute;
        _level.style.left = 5;
        _level.AddToClassList(_ussCommonTextPrimary);
        ExpBar.Add(_level);

        container.Add(ExpBar);
        return container;
    }

    void OnLevelUp()
    {
        _level.text = $"Level {Hero.Level.Value}";
    }

    VisualElement CreateManaGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussManaContainer);

        Color c = _gameManager.GameDatabase.GetColorByName("Mana").Color;
        ManaBar = new(c, "Mana", Hero.CurrentMana, Hero.BaseTotalMana);
        container.Add(ManaBar);

        return container;
    }

}
