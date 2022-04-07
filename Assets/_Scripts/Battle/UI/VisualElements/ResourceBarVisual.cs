using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class ResourceBarVisual : VisualElement
{
    VisualElement _missing;
    VisualElement _interactionResult;
    Label _text;

    string _tweenID;

    public ResourceBarVisual(Color color)
    {
        _missing = new();
        _interactionResult = new();
        _text = new();

        style.backgroundColor = color;

        AddToClassList("resourceBar");
        _missing.AddToClassList("barMissingAmount");
        _interactionResult.AddToClassList("barInteractionResult");
        _text.AddToClassList("barText");

        Add(_missing);
        Add(_interactionResult);
        Add(_text);

        _tweenID = Guid.NewGuid().ToString();
    }

    public void DisplayMissingAmount(int total, int current)
    {
        _missing.style.display = DisplayStyle.Flex;
        float missingPerc = ((float)total - (float)current) / (float)total;
        missingPerc = Mathf.Clamp(missingPerc, 0, 1);
        _missing.style.width = Length.Percent(missingPerc * 100);

        SetText($"{current}/{total}");
    }

    public void DisplayInteractionResult(int total, int current, int value)
    {
        DisplayMissingAmount(total, current);

        // nothing to heal
        if (value > 0 && current >= total)
            return;

        string resultText = "" + (value + current);
        float percent = Mathf.Abs(value) / (float)total * 100;
        // limit percent to current health when damaging
        if (value < 0 && Mathf.Abs(value) >= current)
        {
            resultText = "" + 0;
            percent = current / (float)total * 100;
        }
        // limit percent to missing missing health when healing 
        if (value > 0 && Mathf.Abs(value) >= total - current)
        {
            resultText = "" + total;
            percent = (total - current) / (float)total * 100;
        }

        _interactionResult.style.display = DisplayStyle.Flex;
        _interactionResult.style.width = Length.Percent(percent);
        Color color = Color.black;
        if (value < 0)
        {
            _interactionResult.style.right = Length.Percent(0);
            color = Color.black;
        }
        else
        {
            _interactionResult.style.right = Length.Percent(percent);
            color = Color.white;
        }

        AnimateInteractionResult(color);

        SetText($"{resultText}/{total}");
    }

    public void HideInteractionResult(int total, int current)
    {
        DOTween.Pause(_tweenID);

        _interactionResult.style.display = DisplayStyle.None;
        DisplayMissingAmount(total, current);
    }

    public void SetText(string newText)
    {
        _text.text = newText;
    }

    void AnimateInteractionResult(Color color)
    {
        _interactionResult.style.backgroundColor = color;

        DOTween.ToAlpha(() => _interactionResult.style.backgroundColor.value,
                         x => _interactionResult.style.backgroundColor = x,
                         0f, 0.8f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(_tweenID);
    }

}
