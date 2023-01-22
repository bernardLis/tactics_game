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
    Label _icon;
    AnimationElement _animationElement;
    Label _text;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "spice-element__";
    const string _ussMain = _ussClassName + "main";

    public SpiceElement(int amount)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SpiceElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        Sprite[] animationSprites = null;
        animationSprites = _gameManager.GameDatabase.GetSpiceSprites(amount);
        _icon = new();
        _icon.style.width = 50;
        _icon.style.height = 50;
        _animationElement = new AnimationElement(animationSprites, 100, true);
        _animationElement.PlayAnimation();
        _icon.Add(_animationElement);
        Add(_icon);

        _text = new();
        _text.AddToClassList(_ussCommonTextPrimary);
        _text.text = Amount.ToString();
        _text.style.unityTextAlign = TextAnchor.MiddleCenter;
        _text.style.height = 50;
        _text.style.width = 25;
        Add(_text);

        Amount = 0;
        ChangeAmount(amount);

        float startScale = Random.Range(0.8f, 1f);
        float endScale = Random.Range(1f, 1.2f);
        float duration = Random.Range(1.5f, 3f);

        DOTween.To(x => _icon.transform.scale = x * Vector3.one, startScale, endScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
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

            _text.text = displayAmount.ToString();
            await Task.Delay(delay);
        }

        SwapSprites();
    }

    void SwapSprites() { _animationElement.SwapAnimationSprites(_gameManager.GameDatabase.GetSpiceSprites(Amount)); }
}
