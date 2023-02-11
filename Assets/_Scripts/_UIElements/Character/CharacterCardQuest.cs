using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class CharacterCardQuest : VisualElement
{
    GameManager _gameManager;
    public Character Character;


    CharacterPortraitElement _portrait;

    Label _title;
    StarRankElement _rankElement;

    Label _level;
    VisualElement _levelUpAnimationContainer;
    ResourceBarElement _expBar;

    VisualElement _powerStatContainer;
    VisualElement _armorStatContainer;
    VisualElement _rangeStatContainer;

    StatElement _power;
    StatElement _armor;
    StatElement _range;

    MyButton _powerUpButton;
    MyButton _armorUpButton;
    MyButton _rangeUpButton;

    bool _pointAdded;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "character-card-quest__";
    const string _ussMain = _ussClassName + "main";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussStatGroup = _ussClassName + "stat-group";
    const string _ussStatContainer = _ussClassName + "stat-container";
    const string _ussStatUpButton = _ussClassName + "stat-up-button";


    public event Action OnLeveledUp;
    public CharacterCardQuest(Character character)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardQuestStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;

        AddToClassList(_ussMain);

        _portrait = new CharacterPortraitElement(character);
        Add(_portrait);

        Add(CreateExpGroup());
        Add(CreateStatGroup());
    }

    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        _title = new($"[{Character.Rank.Title}] {Character.CharacterName}");
        _rankElement = new(Character.Rank.Rank, 0.5f);

        // TODO: this should be handled differently.
        IntVariable totalExp = ScriptableObject.CreateInstance<IntVariable>();
        totalExp.SetValue(100);

        _expBar = new(Color.black, "Experience", Character.Experience, totalExp, null, 0, true);

        _level = new Label($"Level {Character.Level.Value}");
        _level.AddToClassList(_ussCommonTextPrimary);
        _level.style.position = Position.Absolute;
        _expBar.Add(_level);

        Character.OnRankChanged += OnRankChanged;
        Character.Experience.OnValueChanged += OnExpValueChanged;
        Character.Level.OnValueChanged += OnLevelUp;

        container.Add(_title);
        container.Add(_expBar);
        return container;
    }

    void OnRankChanged(CharacterRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Character.CharacterName}";
    }

    void OnExpValueChanged(int newValue)
    {
        if (newValue < 100)
            return;

        PlayLevelUpAnimation();
        CreateStatUpButtons();
    }

    void OnLevelUp(int level) { _level.text = $"Level {Character.Level.Value}"; }

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

        GameDatabase db = GameManager.Instance.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), Character.Power);
        _armor = new(db.GetStatIconByName("Armor"), Character.Armor);
        _range = new(db.GetStatIconByName("Speed"), Character.Speed);

        _powerStatContainer.Add(_power);
        _armorStatContainer.Add(_armor);
        _rangeStatContainer.Add(_range);

        container.Add(_powerStatContainer);
        container.Add(_armorStatContainer);
        container.Add(_rangeStatContainer);

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
    }

    void PowerUp()
    {
        if (_pointAdded)
            return;
        BaseStatUp();
        Character.AddPower();
    }

    void ArmorUp()
    {
        if (_pointAdded)
            return;

        BaseStatUp();
        Character.AddArmor();
    }

    void RangeUp()
    {
        if (_pointAdded)
            return;

        BaseStatUp();
        Character.AddRange();
    }

    void BaseStatUp()
    {
        _pointAdded = true;

        _powerUpButton.style.display = DisplayStyle.None;
        _armorUpButton.style.display = DisplayStyle.None;
        _rangeUpButton.style.display = DisplayStyle.None;

        Character.LevelUp();
        OnLeveledUp?.Invoke();
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
