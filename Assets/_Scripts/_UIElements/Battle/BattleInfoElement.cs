using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleInfoElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-info__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLeftMouseIcon = _ussClassName + "left-mouse-icon";
    const string _ussRIcon = _ussClassName + "r-icon";

    GameManager _gameManager;

    public BattleInfoElement(string text, bool isObstacleRotation = false)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleInfoStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        // for now just this...
        VisualElement icon = new();
        if (isObstacleRotation)
            icon.AddToClassList(_ussRIcon);
        else
            icon.AddToClassList(_ussLeftMouseIcon);
        Add(icon);

        Label txt = new(text);
        Add(txt);
    }
}
