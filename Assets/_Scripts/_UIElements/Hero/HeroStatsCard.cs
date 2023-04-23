using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroStatsCard : VisualElement
{
    GameManager _gameManager;

    public Hero Hero;

    public HeroPortraitElement PortraitVisualElement;
    ElementalElement _elementalElement;
    Label _title;
    Label _level;

    StatElement _power;
    StatElement _armor;
    StatElement _speed;

    public ResourceBarElement ExpBar;
    public ResourceBarElement ManaBar;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "hero-stats-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussTopLeftPanel = _ussClassName + "top-left-panel";
    const string _ussTopMiddlePanel = _ussClassName + "top-middle-panel";
    const string _ussTopRightPanel = _ussClassName + "top-right-panel";

    const string _ussExpContainer = _ussClassName + "exp-container";
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
        VisualElement topMiddlePanel = new();
        VisualElement topRightPanel = new();
        topPanel.Add(topLeftPanel);
        topPanel.Add(topMiddlePanel);
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
        _title.text = $"[{rank.Title}] {Hero.HeroName}";
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        Hero.OnRankChanged -= OnRankChanged;
    }

    void PopulateTopLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopLeftPanel);
        PortraitVisualElement = new(Hero, this);
        container.Add(PortraitVisualElement);
    }

    void PopulateTopMiddlePanel(VisualElement container)
    {
        container.AddToClassList(_ussTopMiddlePanel);

        HeroDatabase db = _gameManager.HeroDatabase;
        _power = new(db.GetStatIconByName("Power"), Hero.Power);
        _armor = new(db.GetStatIconByName("Armor"), Hero.Armor);
        _speed = new(db.GetStatIconByName("Speed"), Hero.Speed);

        container.Add(_power);
        container.Add(_armor);
        container.Add(_speed);

    }

    void PopulateTopRightPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopRightPanel);

        _title = new($"[{Hero.Rank.Title}] {Hero.HeroName}");
        container.Add(_title);

        container.Add(CreateManaGroup());
        container.Add(CreateExpGroup());
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

    VisualElement CreateManaGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussManaContainer);

        // TODO: this should be handled differently.
        IntVariable currentMana = ScriptableObject.CreateInstance<IntVariable>();
        currentMana.SetValue(Hero.Mana.GetValue());
        Hero.Mana.OnValueChanged += currentMana.SetValue;

        if (Hero.CurrentMana != null)
            currentMana = Hero.CurrentMana;

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", currentMana, totalValueStat: Hero.Mana);
        container.Add(ManaBar);

        return container;
    }

}
