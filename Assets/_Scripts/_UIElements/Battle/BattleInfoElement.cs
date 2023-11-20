using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleInfoElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-info__";
    const string _ussMain = _ussClassName + "main";
    const string _ussInteraction = _ussClassName + "interaction";
    const string _ussRIcon = _ussClassName + "r-icon";

    GameManager _gameManager;

    GoldElement _purchasePrice;

    public BattleInfoElement(string text, int purchasePrice = 0)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleKeyTooltipStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        // for now just this...
        VisualElement icon = new();
        icon.AddToClassList(_ussInteraction);
        Add(icon);

        Label txt = new(text);
        Add(txt);

        if (purchasePrice > 0)
        {
            _purchasePrice = new(purchasePrice);
            Add(_purchasePrice);
        }
    }

    public void UpdatePurchasePrice(int price)
    {
        if (_purchasePrice != null)
        {
            _purchasePrice.ChangeAmount(price);
            return;
        }

        _purchasePrice = new(price);
        Add(_purchasePrice);
    }
}
