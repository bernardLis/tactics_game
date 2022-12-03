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

        Amount = 0;

        AddToClassList("goldElement");

        _icon = new();
        _icon.style.width = 50;
        _icon.style.height = 50;
        _icon.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.GetCoinSprite(amount));
        Add(_icon);

        _text = new();
        _text.AddToClassList("textPrimary");
        _text.text = Amount.ToString();
        _text.style.width = 60;
        Add(_text);

        if (isClickable)
            MakeClickable();

        ChangeAmount(amount);
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
