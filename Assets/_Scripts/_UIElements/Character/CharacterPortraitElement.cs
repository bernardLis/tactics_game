using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitElement : VisualElement
{
    Character _character;
    VisualElement _frame;
    public CharacterPortraitElement(Character character)
    {
        _character = character;
        AddToClassList("characterPortraitContainer");

        VisualElement portrait = new();
        portrait.AddToClassList("characterPortrait");
        portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);

        _frame = new();
        _frame.AddToClassList("characterPortraitFrame");

        UpdateFrame(character.Rank);
        character.OnRankChanged += UpdateFrame;

        Add(portrait);
        Add(_frame);
    }

    void UpdateFrame(CharacterRank rank)
    {
        _frame.style.backgroundImage = new StyleBackground(rank.PortraitBorder);
    }

}
