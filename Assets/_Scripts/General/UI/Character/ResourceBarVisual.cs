using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using System.Threading.Tasks;

public class ResourceBarVisual : VisualWithTooltip
{
    VisualElement _resourceBar;
    VisualElement _missing;
    VisualElement _interactionResult;
    Label _text;

    int _lastTotalValue;
    int _lastCurrentValue;

    string _tweenID;

    string _tooltipText;

    public ResourceBarVisual(Color color, string tooltipText, int thickness = 0) : base()
    {
        AddToClassList("barContainer");

        _tooltipText = tooltipText;

        _missing = new();
        _interactionResult = new();
        _text = new();


        _resourceBar = new();
        _resourceBar.AddToClassList("resourceBar");
        _resourceBar.style.backgroundColor = color;
        Add(_resourceBar);

        _missing.AddToClassList("barMissingAmount");
        _interactionResult.AddToClassList("barInteractionResult");
        _text.AddToClassList("barText");
        _text.AddToClassList("secondaryText");

        if (thickness != 0)
            style.height = thickness;

        _resourceBar.Add(_missing);
        _resourceBar.Add(_interactionResult);
        _resourceBar.Add(_text);

        _tweenID = Guid.NewGuid().ToString();
    }

    public void DisplayMissingAmount(int total, int current)
    {
        _lastTotalValue = total;
        _lastCurrentValue = current;

        _missing.style.display = DisplayStyle.Flex;
        float missingPercent = (float)current / (float)total;
        missingPercent = Mathf.Clamp(missingPercent, 0, 1);

        float targetWidth = localBound.width * (1 - missingPercent);
        _missing.style.width = targetWidth;

        SetText($"{current}/{total}");
    }

    public async void OnValueChange(int total, int beforeChange, int change)
    {
        HideInteractionResult(total, beforeChange);
        if (change == 0)
            return;
        float missingPercent = ((float)beforeChange + (float)change) / (float)total;
        missingPercent = Mathf.Clamp(missingPercent, 0, 1);

        float targetWidth = localBound.width * (1 - missingPercent);// * 100;
        DOTween.To(() => _missing.style.width.value.value, x => _missing.style.width = x, targetWidth, 1f);

        int current = beforeChange;
        int goal = beforeChange + change;
        goal = Mathf.Clamp(goal, 0, total);
        int delay = Mathf.FloorToInt(1000 / Mathf.Abs(change)); // do it in 1second

        if (change < 0)
        {
            while (current > goal)
            {
                current--;
                SetText($"{current}/{total}");
                await Task.Delay(delay);
            }
        }
        else
        {
            while (current < goal)
            {
                current++;
                SetText($"{current}/{total}");
                await Task.Delay(delay);
            }
        }
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

    public void HideInteractionResult()
    {
        BaseHideInteractionResult();
        DisplayMissingAmount(_lastTotalValue, _lastCurrentValue);
    }

    public void HideInteractionResult(int total, int current)
    {
        BaseHideInteractionResult();
        DisplayMissingAmount(total, current);
    }

    void BaseHideInteractionResult()
    {
        DOTween.Pause(_tweenID);

        _interactionResult.style.display = DisplayStyle.None;
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

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }
}
