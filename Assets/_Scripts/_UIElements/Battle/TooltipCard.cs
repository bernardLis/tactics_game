using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "tooltip-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussBackground = _ussClassName + "background";
    const string _ussTopLeftContainer = _ussClassName + "top-left-container";
    const string _ussTopMiddleContainer = _ussClassName + "top-middle-container";


    protected VisualElement _topLeftContainer;
    protected VisualElement _topRightContainer;


    protected GameManager _gameManager;

    protected void Initialize()
    {
        _gameManager = GameManager.Instance;

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TooltipCardStyles);
        if (ss != null) styleSheets.Add(ss);
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        VisualElement bg = new();
        bg.AddToClassList(_ussBackground);
        Add(bg);

        _topLeftContainer = new();
        _topLeftContainer.AddToClassList(_ussTopLeftContainer);
        _topRightContainer = new();
        _topRightContainer.AddToClassList(_ussTopMiddleContainer);

        Add(_topLeftContainer);
        Add(_topRightContainer);
    }
}
