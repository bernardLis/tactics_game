using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GoldElement : VisualElement
{
    public int Amount;
    Label _icon;
    Label _text;
    public GoldElement(int amount)
    {
        Amount = amount;

        style.flexDirection = FlexDirection.Row;

        _icon = new();
        _icon.style.backgroundImage = new StyleBackground(GameManager.Instance.GameDatabase.GetCoinSprite(amount));
        Add(_icon);

        Label _text = new(amount.ToString());
        _text.AddToClassList("textPrimary");
        Add(_text);
    }

    public async void ChangeAmount(int newValue)
    {

        Debug.Log($"gold element changing the amount {newValue}");
        while (Amount != newValue)
        {
            if (Amount < newValue)
                Amount++;
            if (Amount > newValue)
                Amount--;

            _icon.style.backgroundImage = new StyleBackground(GameManager.Instance.GameDatabase.GetCoinSprite(Amount));
            _text.text = Amount.ToString();
            await Task.Delay(50);

        }


    }
}
