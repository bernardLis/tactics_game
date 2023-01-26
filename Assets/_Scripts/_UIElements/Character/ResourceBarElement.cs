using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using System.Threading.Tasks;

public class ResourceBarElement : ElementWithTooltip
{
    VisualElement _resourceBar;
    VisualElement _missing;
    Label _text;

    int _total;
    int _current;
    bool _isGaining;

    string _tweenID;

    string _tooltipText;

    const string _ussCommonTextSecondary = "common__text-secondary";

    const string _ussClassName = "resource-bar";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussMissing = _ussClassName + "__missing";
    const string _ussBarText = _ussClassName + "__bar-text";

    public ResourceBarElement(Color color, string tooltipText, int total, int current, int thickness = 0, bool isGaining = false) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _total = total;
        _current = current;
        _isGaining = isGaining;

        AddToClassList(_ussContainer);

        _tooltipText = tooltipText;

        _missing = new();
        _text = new();

        _resourceBar = new();
        _resourceBar.AddToClassList(_ussMain);

        if (_isGaining)
            _resourceBar.style.flexDirection = FlexDirection.RowReverse;
        else
            _resourceBar.style.flexDirection = FlexDirection.Row;

        _resourceBar.style.backgroundColor = color;
        Add(_resourceBar);

        _missing.AddToClassList(_ussMissing);
        _text.AddToClassList(_ussBarText);
        _text.AddToClassList(_ussCommonTextSecondary);

        if (thickness != 0)
            style.height = thickness;

        _resourceBar.Add(_missing);
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

    public void SetText(string newText) { _text.text = newText; }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }
}
