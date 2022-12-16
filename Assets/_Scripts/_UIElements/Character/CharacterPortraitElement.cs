using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitElement : VisualElement
{
    Character _character;
    VisualElement _frame;

    const string _ussClassName = "character-portrait";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussFrame = _ussClassName + "__frame";

    public CharacterPortraitElement(Character character)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterPortraitStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _character = character;
        AddToClassList(_ussContainer);

        VisualElement portrait = new();
        portrait.AddToClassList(_ussMain);
        portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);

        _frame = new();
        _frame.AddToClassList(_ussFrame);

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
