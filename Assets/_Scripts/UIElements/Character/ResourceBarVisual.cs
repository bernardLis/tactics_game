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

    int _total;
    int _current;
    bool _isGaining;

    string _tweenID;

    string _tooltipText;

    public ResourceBarVisual(Color color, string tooltipText, int total, int current, int thickness = 0, bool isGaining = false) : base()
    {
        _total = total;
        _current = current;
        _isGaining = isGaining;

        AddToClassList("barContainer");

        _tooltipText = tooltipText;

        _missing = new();
        _interactionResult = new();
        _text = new();

        _resourceBar = new();
        _resourceBar.AddToClassList("resourceBar");

        if (_isGaining)
            _resourceBar.style.flexDirection = FlexDirection.RowReverse;
        else
            _resourceBar.style.flexDirection = FlexDirection.Row;

        _resourceBar.style.backgroundColor = color;
        Add(_resourceBar);

        _missing.AddToClassList("barMissingAmount");
        _interactionResult.AddToClassList("barInteractionResult");
        _text.AddToClassList("barText");
        _text.AddToClassList("textSecondary");

        if (thickness != 0)
            style.height = thickness;

        _resourceBar.Add(_missing);
        _resourceBar.Add(_interactionResult);
        _resourceBar.Add(_text);

        _tweenID = Guid.NewGuid().ToString();

        DisplayMissingAmount();
    }

    public void UpdateBarValues(int total, int current)
    {
        _total = total;
        _current = current;

        DisplayMissingAmount();
    }

    public void DisplayMissingAmount()
    {
        _missing.style.display = DisplayStyle.Flex;

        float missingPercent = (float)_current / (float)_total;
        missingPercent = Mathf.Clamp(missingPercent, 0, 1);

        _missing.style.width = Length.Percent((1 - missingPercent) * 100); //targetWidth;

        SetText($"{_current}/{_total}");
    }

    public void OnTotalChanged(int total)
    {
        _total = total;
        UpdateBarValues(_total, _current);
    }

    public async void OnValueChanged(int change) { await BaseOnValueChanged(change, 1000); }

    public async void OnValueChanged(int change, int totalDelay) { await BaseOnValueChanged(change, totalDelay); }

    async Task BaseOnValueChanged(int change, int totalDelay)
    {
        HideInteractionResult();

        if (change == 0)
            return;

        int goal = Mathf.Clamp(_current + change, 0, _total);
        int delay = Mathf.FloorToInt(1000 / Mathf.Abs(change)); // do it in 1second

        if (change < 0)
            await HandleLose(goal, delay);
        else
            await HandleGain(goal, delay);
    }

    async Task HandleLose(int goal, int delay)
    {
        while (_current > goal)
        {
            _current--;
            DisplayMissingAmount();
            await Task.Delay(delay);
        }
    }

    async Task HandleGain(int goal, int delay)
    {
        while (_current < goal)
        {
            _current++;
            DisplayMissingAmount();
            await Task.Delay(delay);
        }
    }

    public void DisplayInteractionResult(int total, int current, int value)
    {
        UpdateBarValues(total, current);

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
        DOTween.Pause(_tweenID);
        _interactionResult.style.display = DisplayStyle.None;
        UpdateBarValues(_total, _current);
    }

    public void SetText(string newText) { _text.text = newText; }

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
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }
}
