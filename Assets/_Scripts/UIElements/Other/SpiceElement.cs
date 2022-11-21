using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class SpiceElement : VisualElement
{
    GameManager _gameManager;

    public int Amount;
    SpiceColor _spiceColor;
    Label _icon;
    Label _text;

    Sprite[] _animationSprites;
    int _spriteIndex;
    IVisualElementScheduledItem _animationScheduler;

    public SpiceElement(int amount, SpiceColor spiceColor)
    {
        _gameManager = GameManager.Instance;

        // TODO: could be improved
        if (spiceColor == SpiceColor.Yellow)
            _animationSprites = _gameManager.GameDatabase.YellowSpiceAnimationSprites;
        if (spiceColor == SpiceColor.Blue)
            _animationSprites = _gameManager.GameDatabase.BlueSpiceAnimationSprites;
        if (spiceColor == SpiceColor.Red)
            _animationSprites = _gameManager.GameDatabase.RedSpiceAnimationSprites;

        _spiceColor = spiceColor;
        Amount = 0;

        AddToClassList("spiceElement");

        _icon = new();
        _icon.style.backgroundImage = new StyleBackground(_animationSprites[_spriteIndex]);
        _icon.style.width = 50;
        _icon.style.height = 50;
        Add(_icon);

        _text = new();
        _text.AddToClassList("textPrimary");
        _text.text = Amount.ToString();
        Add(_text);

        ChangeAmount(amount);

        _animationScheduler = _icon.schedule.Execute(IdleAnimation).Every(100);
        float startScale = Random.Range(0.8f, 1f);
        float endScale = Random.Range(1f, 1.2f);
        float duration = Random.Range(1.5f, 3f);

        DOTween.To(x => _icon.transform.scale = x * Vector3.one, startScale, endScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void IdleAnimation()
    {
        _icon.style.backgroundImage = new StyleBackground(_animationSprites[_spriteIndex]);
        _spriteIndex++;
        if (_spriteIndex == _animationSprites.Length)
            _spriteIndex = 0;
    }

    public void OnValueChanged(int change) { ChangeAmount(Amount + change); }

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
