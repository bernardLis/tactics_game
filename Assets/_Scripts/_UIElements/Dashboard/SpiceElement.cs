using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;

public class SpiceElement : ChangingValueElement
{
    GameManager _gameManager;

    AnimationElement _animationElement;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "spice-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussValue = _ussClassName + "value";

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
        _animationElement = new AnimationElement(animationSprites, 100, true, true);
        _animationElement.AddToClassList(_ussIcon);
        _animationElement.PlayAnimation();
        Add(_animationElement);

        _text = new();
        _text.AddToClassList(_ussCommonTextPrimary);
        _text.AddToClassList(_ussValue);
        _text.text = Amount.ToString();
        Add(_text);

        Amount = 0;
        ChangeAmount(amount);

        float startScale = Random.Range(0.8f, 1f);
        float endScale = Random.Range(1f, 1.2f);
        float duration = Random.Range(1.5f, 3f);

        DOTween.To(x => _animationElement.transform.scale = x * Vector3.one, startScale, endScale, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
    }

    public void OnValueChanged(int change) { ChangeAmount(Amount + change); }

    protected override void FinishAnimation()
    {
        base.FinishAnimation();
        _animationElement.SwapAnimationSprites(_gameManager.GameDatabase.GetSpiceSprites(Amount));
    }
}
