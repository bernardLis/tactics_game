using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class HeroPortraitElement : VisualElement
{
    const string _ussClassName = "hero-portrait";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussFrame = _ussClassName + "__frame";

    const string _ussContainerSlotted = _ussClassName + "__container-slotted";
    const string _ussMainSlotted = _ussClassName + "__main-slotted";
    const string _ussFrameSlotted = _ussClassName + "__frame-slotted";

    public HeroCardStats Card;
    public Hero Hero;
    VisualElement _portrait;
    VisualElement _frame;

    public HeroPortraitElement(Hero hero, HeroCardStats card = null)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroPortraitStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;
        Card = card;
        AddToClassList(_ussContainer);

        _portrait = new();
        _portrait.AddToClassList(_ussMain);
        _portrait.style.backgroundImage = new StyleBackground(hero.Portrait.Sprite);

        _frame = new();
        _frame.AddToClassList(_ussFrame);

        UpdateFrame(hero.Rank);
        hero.OnRankChanged += UpdateFrame;

        Add(_portrait);
        Add(_frame);
    }

    void UpdateFrame(HeroRank rank) { _frame.style.backgroundImage = new StyleBackground(rank.PortraitBorder); }

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
