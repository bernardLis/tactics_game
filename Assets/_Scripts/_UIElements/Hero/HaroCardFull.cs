
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCardFull : FullScreenElement
{
    const string _ussClassName = "hero-card-full__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopContainer = _ussClassName + "top-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussBottomContainer = _ussClassName + "bottom-container";

    public Hero Hero { get; private set; }

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    public HeroCardFull(Hero hero) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        VisualElement container = new();
        container.AddToClassList(_ussMain);
        _content.Add(container);

        _topContainer = new();
        _topContainer.AddToClassList(_ussTopContainer);
        container.Add(_topContainer);

        _middleContainer = new();
        _middleContainer.AddToClassList(_ussMiddleContainer);
        container.Add(_middleContainer);

        _bottomContainer = new();
        _bottomContainer.AddToClassList(_ussBottomContainer);
        container.Add(_bottomContainer);

        HeroCardStats card = new(Hero);
        card.BlockClick();
        _topContainer.Add(card);

        HeroItemsElement itemsElement = new(Hero);
        _middleContainer.Add(itemsElement);

        HeroAbilitiesElement abilitiesElement = new(Hero);
        _middleContainer.Add(abilitiesElement);

        HeroArmyElement armyElement = new(Hero);
        _bottomContainer.Add(armyElement);

        AddContinueButton();
    }
}