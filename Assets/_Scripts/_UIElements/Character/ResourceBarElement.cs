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
    IntVariable _currentInt;
    FloatVariable _currentFloat;
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
            FloatVariable currentFloatVar = null,
            Stat totalValueStat = null, int thickness = 0, bool isIncreasing = false) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
        if (ss != null)
            styleSheets.Add(ss);

        if (totalValueStat != null)
        {
            _total = totalValueStat.GetValue();
            totalValueStat.OnValueChanged += OnTotalChanged;
        }
        if (totalIntVar != null)
        {
            _total = totalIntVar.Value;
            totalIntVar.OnValueChanged += OnTotalChanged;
        }

        if (currentFloatVar != null)
        {
            _currentFloat = currentFloatVar;
            _displayedAmount = (int)currentFloatVar.Value;
            currentFloatVar.OnValueChanged += OnValueChanged;

        }

        if (currentIntVar != null)
        {
            _currentInt = currentIntVar;
            _displayedAmount = _currentInt.Value;
            currentIntVar.OnValueChanged += OnValueChanged;
        }


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
        int change = Mathf.Abs(newValue - _currentInt.PreviousValue);
        if (change == 0)
            return;

        if (_animation != null)
            _animation.Pause();

        int delay = Mathf.FloorToInt(1000 / change); // do it in 1second

        if (newValue - _currentInt.PreviousValue < 0)
            _animation = schedule.Execute(HandleDecrease).Every(delay);
        else
            _animation = schedule.Execute(HandleIncrease).Every(delay);
    }

    void OnValueChanged(float newValue)
    {
        float change = Mathf.Abs(newValue - _currentFloat.PreviousValue);
        if (change == 0)
            return;

        if (_animation != null)
            _animation.Pause();

        int delay = Mathf.FloorToInt(1000 / change); // do it in 1second

        if (newValue - _currentFloat.PreviousValue < 0)
            _animation = schedule.Execute(HandleDecrease).Every(delay);
        else
            _animation = schedule.Execute(HandleIncrease).Every(delay);
    }


    void HandleDecrease()
    {
        if (_currentInt != null && _currentInt.Value >= _displayedAmount)
        {
            _animation.Pause();
            return;
        }

        if (_currentFloat != null && _currentFloat.Value >= _displayedAmount)
        {
            _animation.Pause();
            return;
        }

        _displayedAmount--;
        DisplayMissingAmount();
    }

    void HandleIncrease()
    {
        if (_currentInt != null && _currentInt.Value <= _displayedAmount)
        {
            _animation.Pause();
            return;
        }

        if (_currentFloat != null && _currentFloat.Value <= _displayedAmount)
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
