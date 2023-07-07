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

    const string _ussContainerSmall = _ussClassName + "__container-small";
    const string _ussMainSmall = _ussClassName + "__main-small";
    const string _ussFrameSmall = _ussClassName + "__frame-small";

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

    public void SmallPortrait()
    {
        AddToClassList(_ussContainerSmall);
        _portrait.AddToClassList(_ussMainSmall);
        _frame.AddToClassList(_ussFrameSmall);
    }

    public void NotSmallPortrait()
    {
        RemoveFromClassList(_ussContainerSmall);
        _portrait.RemoveFromClassList(_ussMainSmall);
        _frame.RemoveFromClassList(_ussFrameSmall);
    }
}
