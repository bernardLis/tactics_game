using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitVisualElement : VisualElement
{
    Character _character;
    public CharacterPortraitVisualElement(Character character)
    {
        _character = character;
        AddToClassList("characterPortraitContainer");
        
        VisualElement portrait = new();
        portrait.AddToClassList("characterPortrait");
        portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);

        VisualElement frame = new();
        frame.AddToClassList("characterPortraitFrame");

        // HERE:ranks request frame depending on character rank

        Add(portrait);
        Add(frame);
    }

}
