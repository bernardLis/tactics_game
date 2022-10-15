using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GoldElement : VisualElement
{
    GameManager _gameManager;
    public int Amount;
    Label _icon;
    Label _text;

    bool _clicked;
    public GoldElement(int amount, bool isClickable = false)
    {
        _gameManager = GameManager.Instance;

        Amount = amount;
        AddToClassList("goldElement");

        _icon = new();
        _icon.style.width = 50;
        _icon.style.height = 50;
        _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(amount));
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
        _gameManager.ChangeGoldValue(Amount);
        ChangeAmount(0);
    }

    public async void ChangeAmount(int newValue)
    {
        if (newValue == Amount)
            return;

        int displayAmount = Amount;
        Amount = newValue;

        int steps = Mathf.Abs(displayAmount - newValue);
        int delay = 1000 / steps;


        Debug.Log($"gold element changing the amount {newValue}");

        while (displayAmount != newValue)
        {
            if (displayAmount < newValue)
                displayAmount++;
            if (displayAmount > newValue)
                displayAmount--;

            _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(Amount));
            _text.text = displayAmount.ToString();
            await Task.Delay(delay);
        }
    }
}
