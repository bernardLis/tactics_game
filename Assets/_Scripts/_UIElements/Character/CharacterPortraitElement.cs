using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitElement : VisualElement
{
    public CharacterCard Card;
    public Character Character;
    VisualElement _portrait;
    VisualElement _frame;

    const string _ussClassName = "character-portrait";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussFrame = _ussClassName + "__frame";

    const string _ussContainerSlotted = _ussClassName + "__container-slotted";
    const string _ussMainSlotted = _ussClassName + "__main-slotted";
    const string _ussFrameSlotted = _ussClassName + "__frame-slotted";


    public CharacterPortraitElement(Character character, CharacterCard card = null)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterPortraitStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;
        Card = card;
        AddToClassList(_ussContainer);

        _portrait = new();
        _portrait.AddToClassList(_ussMain);
        _portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);

        _frame = new();
        _frame.AddToClassList(_ussFrame);

        UpdateFrame(character.Rank);
        character.OnRankChanged += UpdateFrame;

        Add(_portrait);
        Add(_frame);
    }

    void UpdateFrame(CharacterRank rank)
    {
        _frame.style.backgroundImage = new StyleBackground(rank.PortraitBorder);
    }

    public void Slotted()
    {
        AddToClassList(_ussContainerSlotted);
        _portrait.AddToClassList(_ussMainSlotted);
        _frame.AddToClassList(_ussFrameSlotted);
    }

    public void Unslotted()
    {
        RemoveFromClassList(_ussContainerSlotted);
        _portrait.RemoveFromClassList(_ussMainSlotted);
        _frame.RemoveFromClassList(_ussFrameSlotted);
    }
}
