using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardExp : VisualElement
{
    GameManager _gameManager;
    public Character Character;


    CharacterPortraitElement _portrait;

    Label _title;
    StarRankElement _rankElement;

    Label _level;
    VisualElement _levelUpAnimationContainer;
    ResourceBarElement _expBar;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "character-card-exp__";
    const string _ussMain = _ussClassName + "main";
    const string _ussExpContainer = _ussClassName + "exp-container";

    public CharacterCardExp(Character character)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;

        AddToClassList(_ussMain);

        _portrait = new CharacterPortraitElement(character);
        Add(_portrait);

        Add(CreateExpGroup());
    }

    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        _title = new($"[{Character.Rank.Title}] {Character.CharacterName}");
        _rankElement = new(Character.Rank.Rank, 0.5f);

        _level = new Label($"Level {Character.Level}");
        _level.AddToClassList(_ussCommonTextPrimary);

        _expBar = new(Color.black, "Experience", 100, Character.Experience, 0, true);

        Character.OnRankChanged += OnRankChanged;
        Character.OnCharacterExpGain += OnExpChange;
        Character.OnCharacterLevelUp += OnLevelUp;

        container.Add(_title);
        container.Add(_level);
        container.Add(_expBar);
        return container;
    }

    void OnRankChanged(CharacterRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Character.CharacterName}";
    }

    void OnExpChange(int expGain) { _expBar.OnValueChanged(expGain, 3000); }

    void OnLevelUp()
    {
        _level.text = $"Level {Character.Level}";
        PlayLevelUpAnimation();
    }

    public void PlayLevelUpAnimation()
    {
        Sprite[] animationSprites = _gameManager.GameDatabase.LevelUpAnimationSprites;

        _levelUpAnimationContainer = new();
        _levelUpAnimationContainer.style.position = Position.Absolute;
        _levelUpAnimationContainer.style.width = Length.Percent(100);
        _levelUpAnimationContainer.style.height = Length.Percent(100);
        _levelUpAnimationContainer.Add(new AnimationElement(animationSprites, 100, false));
        Add(_levelUpAnimationContainer);

        // TODO: scale it
    }
}
