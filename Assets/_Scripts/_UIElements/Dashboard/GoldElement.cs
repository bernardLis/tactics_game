using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GoldElement : ElementWithTooltip
{
    GameManager _gameManager;
    public int Amount;
    Label _icon;
    Label _text;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "gold-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussValue = _ussClassName + "value";

    public GoldElement(int amount)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GoldElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Amount = 0;

        AddToClassList(_ussMain);

        _icon = new();
        _icon.AddToClassList(_ussIcon);
        _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(amount));
        Add(_icon);

        _text = new();
        _text.AddToClassList(_ussCommonTextPrimary);
        _text.AddToClassList(_ussValue);
        _text.text = Amount.ToString();
        Add(_text);

        ChangeAmount(amount);
    }

    public async void ChangeAmount(int newValue) { await AwaitableChangeAmount(newValue); }

    public async Task AwaitableChangeAmount(int newValue)
    {
        if (newValue == Amount)
            return;

        int displayAmount = Amount;
        Amount = newValue;

        int step = 1;
        int change = Mathf.Abs(displayAmount - newValue);

        // TODO: there has to be a better way
        if (change >= 1000)
            step = 10;
        if (change >= 10000)
            step = 100;
        if (change >= 100000)
            step = 1000;

        int numberOfSteps = Mathf.FloorToInt(change / step);
        int delay = 1000 / numberOfSteps;
        while (displayAmount != newValue)
        {
            if (displayAmount < newValue)
                displayAmount += step;
            if (displayAmount > newValue)
                displayAmount -= step;

            _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(Amount));
            _text.text = displayAmount.ToString();
            await Task.Delay(delay);
        }
    }
}
