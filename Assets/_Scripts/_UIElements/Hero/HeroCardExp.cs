using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class HeroCardExp : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "hero-card-exp__";
    const string _ussMain = _ussClassName + "main";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussManaContainer = _ussClassName + "mana-container";
    const string _ussStatGroup = _ussClassName + "stat-group";
    const string _ussStatContainer = _ussClassName + "stat-container";
    const string _ussStatUpButton = _ussClassName + "stat-up-button";

    GameManager _gameManager;
    public Hero Hero;

    HeroCardMini _heroCardMini;

    Label _title;
    StarRankElement _rankElement;

    Label _level;
    VisualElement _levelUpAnimationContainer;
    ResourceBarElement _expBar;
    ResourceBarElement _manaBar;

    VisualElement _powerStatContainer;
    VisualElement _armorStatContainer;
    VisualElement _rangeStatContainer;

    StatElement _power;
    StatElement _armor;
    StatElement _range;

    MyButton _powerUpButton;
    MyButton _armorUpButton;
    MyButton _rangeUpButton;

    public event Action OnPointAdded;
    public HeroCardExp(Hero hero)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        AddToClassList(_ussMain);

        _heroCardMini = new HeroCardMini(hero);
        Add(_heroCardMini);

        Add(CreateMiddlePanel());
        Add(CreateStatGroup());
    }

    VisualElement CreateMiddlePanel()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        _title = new($"[{Hero.Rank.Title}] {Hero.HeroName}");
        _rankElement = new(Hero.Rank.Rank, 0.5f);

        // TODO: this should be handled differently.
        IntVariable totalExp = ScriptableObject.CreateInstance<IntVariable>();
        totalExp.SetValue(100);

        _expBar = new(Color.black, "Experience", Hero.Experience, totalExp,
                null, thickness: 0, isIncreasing: true);

        _level = new Label($"Level {Hero.Level.Value}");
        _level.AddToClassList(_ussCommonTextPrimary);
        _level.style.position = Position.Absolute;
        _expBar.Add(_level);

        Hero.OnRankChanged += OnRankChanged;
        Hero.Experience.OnValueChanged += OnExpValueChanged;
        Hero.Level.OnValueChanged += OnLevelUp;

        container.Add(_title);
        container.Add(_expBar);
        container.Add(CreateManaGroup());
        container.Add(HandleUnavailability());

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

        _manaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", currentMana, totalValueStat: Hero.Mana);
        container.Add(_manaBar);

        return container;
    }

    void OnRankChanged(HeroRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Hero.HeroName}";
    }

    void OnExpValueChanged(int newValue)
    {
        if (newValue < 100)
            return;

        EnableStatUpButtons();
        PlayLevelUpAnimation();
    }

    void OnLevelUp(int level) { _level.text = $"Level {Hero.Level.Value}"; }

    VisualElement CreateStatGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussStatGroup);

        _powerStatContainer = new();
        _armorStatContainer = new();
        _rangeStatContainer = new();
        _powerStatContainer.AddToClassList(_ussStatContainer);
        _armorStatContainer.AddToClassList(_ussStatContainer);
        _rangeStatContainer.AddToClassList(_ussStatContainer);

        HeroDatabase db = GameManager.Instance.HeroDatabase;
        _power = new(db.GetStatIconByName("Power"), Hero.Power);
        _armor = new(db.GetStatIconByName("Armor"), Hero.Armor);
        _range = new(db.GetStatIconByName("Speed"), Hero.Speed);

        _powerStatContainer.Add(_power);
        _armorStatContainer.Add(_armor);
        _rangeStatContainer.Add(_range);

        container.Add(_powerStatContainer);
        container.Add(_armorStatContainer);
        container.Add(_rangeStatContainer);

        CreateStatUpButtons();

        return container;
    }

    void CreateStatUpButtons()
    {
        _powerUpButton = new(null, _ussStatUpButton, PowerUp);
        _armorUpButton = new(null, _ussStatUpButton, ArmorUp);
        _rangeUpButton = new(null, _ussStatUpButton, RangeUp);

        _powerStatContainer.Add(_powerUpButton);
        _armorStatContainer.Add(_armorUpButton);
        _rangeStatContainer.Add(_rangeUpButton);

        DisableStatUpButtons();
    }

    void PowerUp()
    {
        Hero.AddPower();
        BaseStatUp();
    }

    void ArmorUp()
    {
        Hero.AddArmor();
        BaseStatUp();
    }

    void RangeUp()
    {
        Hero.AddSpeed();
        BaseStatUp();
    }

    void BaseStatUp()
    {
        OnPointAdded?.Invoke();
        if (Hero.LevelUpPointsLeft <= 0)
            DisableStatUpButtons();
    }

    void EnableStatUpButtons()
    {
        _powerUpButton.SetEnabled(true);
        _armorUpButton.SetEnabled(true);
        _rangeUpButton.SetEnabled(true);
    }

    void DisableStatUpButtons()
    {
        _powerUpButton.SetEnabled(false);
        _armorUpButton.SetEnabled(false);
        _rangeUpButton.SetEnabled(false);
    }


    public void PlayLevelUpAnimation()
    {
        Sprite[] animationSprites = _gameManager.GameDatabase.LevelUpAnimationSprites;
        VisualElement container = new();
        container.style.width = 200;
        container.style.height = 200;
        container.style.position = Position.Absolute;

        AnimationElement el = new AnimationElement(animationSprites, 100, false);
        container.Add(el);

        Add(container);
        el.PlayAnimation();
        el.OnAnimationFinished += () => Remove(container);
    }

    VisualElement HandleUnavailability()
    {
        if (!Hero.IsUnavailable())
            return new VisualElement();

        Injury i = Hero.GetActiveInjury();
        string injuryName = Helpers.ParseScriptableObjectCloneName(i.name);
        Label l = new($"{injuryName}, : {i.GetTotalInjuryTimeInSeconds()}s");
        l.style.whiteSpace = WhiteSpace.Normal;
        return l;
    }
}
