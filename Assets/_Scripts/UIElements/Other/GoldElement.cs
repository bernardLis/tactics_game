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

    bool _clicked;
    public GoldElement(int amount, bool isClickable = false)
    {
        Amount = amount;
        AddToClassList("goldElement");

        _icon = new();
        _icon.style.width = 50;
        _icon.style.height = 50;
        _icon.style.backgroundImage = new StyleBackground(GameManager.Instance.GameDatabase.GetCoinSprite(amount));
        Add(_icon);

        _text = new(amount.ToString());
        _text.AddToClassList("textPrimary");
        Add(_text);

        if (isClickable)
            MakeClickable();
    }


    public void MakeClickable()
    {
        RegisterCallback<PointerUpEvent>(OnClick);
        AnimateClickablity();
    }

    async void AnimateClickablity()
    {
        while (!_clicked)
        {
            ToggleInClassList("goldElementClickable");
            await Task.Delay(1000);
        }
    }

    void OnClick(PointerUpEvent e)
    {
        _clicked = true;
        RunManager.Instance.ChangeGoldValue(Amount);
        ChangeAmount(0);
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
            await Task.Delay(100);
        }


    }
}
