
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCardFull : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "hero-card-full__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopContainer = _ussClassName + "top-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussBottomContainer = _ussClassName + "bottom-container";

    GameManager _gameManager;

    public Hero Hero { get; private set; }

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    public HeroCardFull(Hero hero, VisualElement root) : base()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardFullStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root);

        Hero = hero;

        AddToClassList(_ussMain);

        _topContainer = new();
        _topContainer.AddToClassList(_ussTopContainer);
        Add(_topContainer);
        HeroCardStats card = new(Hero);
        _topContainer.Add(card);

        _middleContainer = new();
        _middleContainer.AddToClassList(_ussMiddleContainer);
        Add(_middleContainer);

        HeroItemsElement itemsElement = new(Hero);
        _middleContainer.Add(itemsElement);

        HeroAbilitiesElement abilitiesElement = new(Hero);
        _middleContainer.Add(abilitiesElement);

        _bottomContainer = new();
        _bottomContainer.AddToClassList(_ussBottomContainer);
        Add(_bottomContainer);
        HeroArmyElement armyElement = new(Hero, false);
        _bottomContainer.Add(armyElement);

        AddBackButton();
    }
}
