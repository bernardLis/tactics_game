using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HeroCardExp : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "hero-card-exp__";
    const string _ussMain = _ussClassName + "main";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussManaContainer = _ussClassName + "mana-container";
    const string _ussStatGroup = _ussClassName + "stat-group";
    const string _ussStatGroupAnimation = _ussClassName + "stat-group-animation";

    const string _ussStatContainer = _ussClassName + "stat-container";
    const string _ussStatUpButton = _ussClassName + "stat-up-button";
    const string _ussStatUpButtonAnimation = _ussClassName + "stat-up-button-animation";


    GameManager _gameManager;
    AudioManager _audioManager;
    public Hero Hero;

    public HeroCardMini HeroCardMini;

    Label _title;
    StarRankElement _rankElement;

    Label _level;
    ResourceBarElement _expBar;
    ResourceBarElement _manaBar;

    VisualElement _statGroupContainer;
    VisualElement _powerStatContainer;
    VisualElement _armorStatContainer;
    VisualElement _rangeStatContainer;

    StatElement _power;
    StatElement _armor;
    StatElement _range;

    MyButton _powerUpButton;
    MyButton _armorUpButton;
    MyButton _rangeUpButton;

    bool _statUpButtonsEnabled;

    IVisualElementScheduledItem _pulsatingButtons;
    IVisualElementScheduledItem _pulsatingButtons1;

    public event Action OnPointAdded;
    public HeroCardExp(Hero hero)
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        AddToClassList(_ussMain);

        HeroCardMini = new HeroCardMini(hero);
        Add(HeroCardMini);

        Add(CreateMiddlePanel());
        Add(CreateStatGroup());
    }

    VisualElement CreateMiddlePanel()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        _title = new($"[{Hero.Rank.Title}] {Hero.HeroName}");
        _rankElement = new(Hero.Rank.Rank, 0.5f);

        _expBar = new(Color.gray, "Experience", Hero.Experience, Hero.ExpForNextLevel,
                null, thickness: 0, isIncreasing: false);

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
        if (newValue < Hero.GetExpForNextLevel())
            return;

        EnableStatUpButtons();
        PlayLevelUpAnimation();
    }

    void OnLevelUp(int level)
    {
        _expBar.ChangeValueNoAnimation(0);
        _level.text = $"Level {Hero.Level.Value}";
    }

    public void LeveledUp()
    {
        EnableStatUpButtons();
        PlayLevelUpAnimation();

    }

    VisualElement CreateStatGroup()
    {
        _statGroupContainer = new();
        _statGroupContainer.AddToClassList(_ussStatGroup);

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

        _statGroupContainer.Add(_powerStatContainer);
        _statGroupContainer.Add(_armorStatContainer);
        _statGroupContainer.Add(_rangeStatContainer);

        CreateStatUpButtons();

        return _statGroupContainer;
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
        _audioManager.PlayUI("Point Added");
        OnPointAdded?.Invoke();
        DisableStatUpButtons();
        Hero.LevelUp();
    }

    void EnableStatUpButtons()
    {
        if (_statUpButtonsEnabled) return;
        _statUpButtonsEnabled = true;

        _powerUpButton.SetEnabled(true);
        _armorUpButton.SetEnabled(true);
        _rangeUpButton.SetEnabled(true);
        _pulsatingButtons = this.schedule.Execute(() =>
         {
             _statGroupContainer.AddToClassList(_ussStatGroupAnimation);
             _powerUpButton.AddToClassList(_ussStatUpButtonAnimation);
             _armorUpButton.AddToClassList(_ussStatUpButtonAnimation);
             _rangeUpButton.AddToClassList(_ussStatUpButtonAnimation);

         }).Every(2000);

        _pulsatingButtons1 = this.schedule.Execute(() =>
        {
            _statGroupContainer.RemoveFromClassList(_ussStatGroupAnimation);

            _powerUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);
            _armorUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);
            _rangeUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);
        }).Every(2000).StartingIn(1000);
    }

    void DisableStatUpButtons()
    {
        _statUpButtonsEnabled = false;

        if (_pulsatingButtons != null) _pulsatingButtons.Pause();
        if (_pulsatingButtons1 != null) _pulsatingButtons1.Pause();

        _statGroupContainer.RemoveFromClassList(_ussStatGroupAnimation);

        _powerUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);
        _armorUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);
        _rangeUpButton.RemoveFromClassList(_ussStatUpButtonAnimation);

        //      _powerUpButton.transform.scale = Vector3.one;
        //    _armorUpButton.transform.scale = Vector3.one;
        //  _rangeUpButton.transform.scale = Vector3.one;

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
}
