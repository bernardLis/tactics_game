using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
public class ResourceBarVisual : VisualElement
{
    VisualElement _missing;
    VisualElement _interactionResult;
    Label _text;

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
        Debug.Log("DisplayInteractionResult is called");
        DisplayMissingAmount(total, current);
        _interactionResult.style.display = DisplayStyle.Flex;

        float percent = value / (float)total * 100;
        if (value >= current)
            percent = current / (float)total * 100;

        // reset right
        _interactionResult.style.right = Length.Percent(0);
        _interactionResult.style.width = Length.Percent(value);
        AnimateInteractionResult();

        SetText($"{current - value}/{total}");
    }

    public void SetText(string newText)
    {
        _text.text = newText;
    }

    void AnimateInteractionResult()
    {
        _interactionResult.style.backgroundColor = Color.black;

        DOTween.ToAlpha(() => _interactionResult.style.backgroundColor.value,
                         x => _interactionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo);
        //.SetId(_missingHealthTweenID);
    }



}
