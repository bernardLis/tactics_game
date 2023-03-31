using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCardElement : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "hero-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopContainer = _ussClassName + "top-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussBottomContainer = _ussClassName + "bottom-container";

    GameManager _gameManager;

    public MapHero MapHero { get; private set; }

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    public HeroCardElement(MapHero mapHero, VisualElement root, DraggableArmies draggableArmies) : base()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root);
        _gameManager.ToggleTimer(false);

        MapHero = mapHero;

        AddToClassList(_ussMain);

        _topContainer = new();
        _topContainer.AddToClassList(_ussTopContainer);
        Add(_topContainer);
        CharacterCard card = new(mapHero.Character);
        _topContainer.Add(card);

        _middleContainer = new();
        _middleContainer.AddToClassList(_ussMiddleContainer);
        Add(_middleContainer);

        HeroItemsElement itemsElement = new(mapHero);
        _middleContainer.Add(itemsElement);

        HeroAbilitiesElement abilitiesElement = new(mapHero);
        _middleContainer.Add(abilitiesElement);

        _bottomContainer = new();
        _bottomContainer.AddToClassList(_ussBottomContainer);
        Add(_bottomContainer);
        HeroCastleElement armyElement = new(mapHero.Character, false);
        _bottomContainer.Add(armyElement);

        AddBackButton();

        draggableArmies.Initialize();
    }
}
