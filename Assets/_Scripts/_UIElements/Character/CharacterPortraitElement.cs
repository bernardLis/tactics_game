using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitElement : VisualElement
{
    Character _character;
    VisualElement _frame;

    const string ussClassName = "character-portrait";
    const string ussContainer = ussClassName + "__container";
    const string ussMain = ussClassName + "__main";
    const string ussFrame = ussClassName + "__frame";

    public CharacterPortraitElement(Character character)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterPortraitStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _character = character;
        AddToClassList(ussContainer);

        VisualElement portrait = new();
        portrait.AddToClassList(ussMain);
        portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);

        _frame = new();
        _frame.AddToClassList(ussFrame);

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
