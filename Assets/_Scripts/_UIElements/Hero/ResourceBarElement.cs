using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ResourceBarElement : ElementWithTooltip
{

    const string _ussCommonTextSecondary = "common__text-secondary";

    const string _ussClassName = "resource-bar";
    const string _ussContainer = _ussClassName + "__container";
    const string _ussMain = _ussClassName + "__main";
    const string _ussMissing = _ussClassName + "__missing";
    const string _ussBarText = _ussClassName + "__bar-text";

    public VisualElement ResourceBar;
    public VisualElement MissingBar;
    Label _text;

    int _displayedAmount;
    IntVariable _currentInt;
    IntVariable _totalInt;
    bool _isIncreasing;

    IVisualElementScheduledItem _animation;

    string _tooltipText;

    Color _color;
    int _valueChangeDelay;

    public event Action OnAnimationFinished;

    public ResourceBarElement(Color color, string tooltipText,
            IntVariable currentIntVar, IntVariable totalIntVar,
            int thickness = 0, bool isIncreasing = false,
            int valueChangeDelayMs = 1000) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _color = color;
        _valueChangeDelay = valueChangeDelayMs;

        _totalInt = totalIntVar;
        totalIntVar.OnValueChanged += OnTotalChanged;
        RegisterCallback<DetachFromPanelEvent>((evt) => totalIntVar.OnValueChanged -= OnTotalChanged);
        RegisterCallback<DetachFromPanelEvent>((evt) => Debug.Log($"bar detached"));

        _currentInt = currentIntVar;
        _displayedAmount = _currentInt.Value;
        currentIntVar.OnValueChanged += OnValueChanged;
        RegisterCallback<DetachFromPanelEvent>((evt) => currentIntVar.OnValueChanged -= OnValueChanged);

        AddToClassList(_ussContainer);

        _tooltipText = tooltipText;

        MissingBar = new();
        _text = new();

        ResourceBar = new();
        ResourceBar.AddToClassList(_ussMain);

        _isIncreasing = isIncreasing;
        if (_isIncreasing)
            ResourceBar.style.flexDirection = FlexDirection.Row;
        else
            ResourceBar.style.flexDirection = FlexDirection.RowReverse;

        ResourceBar.style.backgroundColor = color;
        Add(ResourceBar);

        MissingBar.AddToClassList(_ussMissing);
        _text.AddToClassList(_ussBarText);
        _text.AddToClassList(_ussCommonTextSecondary);

        if (thickness != 0)
            style.height = thickness;

        ResourceBar.Add(MissingBar);
        ResourceBar.Add(_text);

        DisplayMissingAmount();
    }

    public void UpdateTrackedVariables(IntVariable current, IntVariable total)
    {
        _currentInt.OnValueChanged -= OnValueChanged;
        _totalInt.OnValueChanged -= OnTotalChanged;

        _currentInt = current;
        _displayedAmount = _currentInt.Value;
        current.OnValueChanged += OnValueChanged;

        _totalInt = total;
        total.OnValueChanged += OnTotalChanged;

        DisplayMissingAmount();
    }

    public void OnTotalChanged(int total)
    {
        DisplayMissingAmount();
    }

    public void DisplayMissingAmount()
    {
        MissingBar.style.display = DisplayStyle.Flex;

        float missingPercent = (float)_displayedAmount / (float)_totalInt.Value;
        missingPercent = Mathf.Clamp(missingPercent, 0, 1);

        MissingBar.style.width = Length.Percent((1 - missingPercent) * 100);

        SetText($"{_displayedAmount}/{_totalInt.Value}");
    }

    public void SetText(string newText) { _text.text = newText; }

    public void ChangeValueNoAnimation(int value)
    {
        _displayedAmount = value;
        DisplayMissingAmount();
        _currentInt.OnValueChanged -= OnValueChanged;
        _currentInt.SetValue(value);
        _currentInt.OnValueChanged += OnValueChanged;
    }

    void OnValueChanged(int newValue)
    {
        int change = Mathf.Abs(newValue - _currentInt.PreviousValue);
        if (change == 0)
            return;

        Helpers.DisplayTextOnElement(null, this, $"{change}", _color);

        if (_animation != null)
        {
            _animation.Pause();
            _displayedAmount = _currentInt.PreviousValue;
            DisplayMissingAmount();
        }

        int delay = Mathf.FloorToInt(_valueChangeDelay / change); // do it in 1second

        if (newValue - _currentInt.PreviousValue < 0)
            _animation = schedule.Execute(HandleDecrease).Every(delay);
        else
            _animation = schedule.Execute(HandleIncrease).Every(delay);
    }

    void HandleDecrease()
    {
        if (_currentInt.Value >= _displayedAmount)
        {
            _animation.Pause();
            OnAnimationFinished?.Invoke();
            return;
        }

        _displayedAmount--;
        DisplayMissingAmount();
    }

    void HandleIncrease()
    {
        if (_currentInt.Value <= _displayedAmount)
        {
            _animation.Pause();
            OnAnimationFinished?.Invoke();
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

    public void HideText() { _text.style.display = DisplayStyle.None; }
}
