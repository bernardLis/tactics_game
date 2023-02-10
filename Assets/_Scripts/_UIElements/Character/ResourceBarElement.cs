using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ResourceBarElement : ElementWithTooltip
{
    VisualElement _resourceBar;
    VisualElement _missing;
    Label _text;

    int _total;
    int _displayedAmount;
    IntVariable _current;
    bool _isIncreasing;

    IVisualElementScheduledItem _animation;

    string _tooltipText;

    const string _ussCommonTextSecondary = "common__text-secondary";

    const string _ussClassName = "resource-bar";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussMissing = _ussClassName + "__missing";
    const string _ussBarText = _ussClassName + "__bar-text";

    public ResourceBarElement(Color color, string tooltipText,
            IntVariable currentIntVar = null, IntVariable totalIntVar = null,
            Stat stat = null, int thickness = 0, bool isIncreasing = false) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
        if (ss != null)
            styleSheets.Add(ss);

        if (stat != null)
        {
            _total = stat.GetValue();
            stat.OnValueChanged += OnTotalChanged;
        }
        if (totalIntVar != null)
        {
            _total = totalIntVar.Value;
            totalIntVar.OnValueChanged += OnTotalChanged;
        }

        _current = currentIntVar;
        _displayedAmount = _current.Value;
        currentIntVar.OnValueChanged += OnValueChanged;

        _isIncreasing = isIncreasing;

        AddToClassList(_ussContainer);

        _tooltipText = tooltipText;

        _missing = new();
        _text = new();

        _resourceBar = new();
        _resourceBar.AddToClassList(_ussMain);

        if (_isIncreasing)
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

        DisplayMissingAmount();
    }

    public void OnTotalChanged(int total)
    {
        _total = total;
        DisplayMissingAmount();
    }

    public void DisplayMissingAmount()
    {
        _missing.style.display = DisplayStyle.Flex;

        float missingPercent = (float)_displayedAmount / (float)_total;
        missingPercent = Mathf.Clamp(missingPercent, 0, 1);

        _missing.style.width = Length.Percent((1 - missingPercent) * 100);

        SetText($"{_displayedAmount}/{_total}");
    }

    public void SetText(string newText) { _text.text = newText; }

    void OnValueChanged(int newValue)
    {
        int change = Mathf.Abs(newValue - _current.PreviousValue);
        Debug.Log($"on value changed, change: {change}");
        if (change == 0)
            return;

        if (_animation != null)
            _animation.Pause();

        int delay = Mathf.FloorToInt(1000 / change); // do it in 1second

        if (newValue - _current.PreviousValue < 0)
            _animation = schedule.Execute(HandleDecrease).Every(delay);
        else
            _animation = schedule.Execute(HandleIncrease).Every(delay);
    }

    void HandleDecrease()
    {
        if (_current.Value == _displayedAmount)
        {
            _animation.Pause();
            return;
        }

        _displayedAmount--;
        DisplayMissingAmount();
    }

    void HandleIncrease()
    {
        if (_current.Value == _displayedAmount)
        {
            _animation.Pause();
            return;
        }

        _displayedAmount++;
        DisplayMissingAmount();
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }
}
