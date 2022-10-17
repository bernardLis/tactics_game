using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardMini : VisualWithTooltip
{
    public Character Character;
    public CharacterCardMini(Character character)
    {
        Character = character;
        AddToClassList("characterCardMini");
        style.backgroundImage = new StyleBackground(character.Portrait);
    }


    protected override void DisplayTooltip()
    {
        HideTooltip();
        CharacterCardExtended tooltip = new CharacterCardExtended(Character);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
