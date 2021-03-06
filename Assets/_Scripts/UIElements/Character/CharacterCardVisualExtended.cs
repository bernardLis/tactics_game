using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardVisualExtended : CharacterCardVisual
{

    Label _level;
    Label _exp;

    public CharacterCardVisualExtended(Character character) : base(character, false)
    {
        BaseCharacterCardVisualExtended(character);
    }
    public CharacterCardVisualExtended(CharacterStats stats) : base(stats, false)
    {
        BaseCharacterCardVisualExtended(stats.Character);
    }

    void BaseCharacterCardVisualExtended(Character character)
    {
        style.width = Length.Percent(50);
        _characteristics.Add(CreateExpGroup(character));
    }

    VisualElement CreateExpGroup(Character character)
    {
        VisualElement container = new();
        container.style.alignSelf = Align.Center;

        VisualElement el = new();
        el.style.flexDirection = FlexDirection.Row;

        _level = new();
        _exp = new();

        _level.AddToClassList("secondaryText");
        _exp.AddToClassList("secondaryText");

        _level.text = $"Level {character.Level}";
        _exp.text = $"Exp: {character.Experience}/100";

        el.Add(_level);
        el.Add(_exp);

        container.Add(el);

        return container;
    }


}
