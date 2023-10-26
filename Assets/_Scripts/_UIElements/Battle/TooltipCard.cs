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

    }
}
