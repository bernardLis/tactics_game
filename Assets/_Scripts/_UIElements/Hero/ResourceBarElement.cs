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

    GameManager _gameManager;

    public VisualElement ResourceBar;
    public VisualElement MissingBar;
    Label _text;

    int _displayedAmount;
    IntVariable _currentInt;
    IntVariable _totalInt;
    Stat _totalStat;

    IVisualElementScheduledItem _animation;

    string _tooltipText;

    Color _color;
    int _valueChangeDelay;

    public event Action OnAnimationFinished;

    public ResourceBarElement(Color color, string tooltipText,
            IntVariable currentIntVar,
            IntVariable totalIntVar = null, Stat totalStat = null,
            int valueChangeDelayMs = 1000) : base()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _color = color;
        _valueChangeDelay = valueChangeDelayMs;

        _tooltipText = tooltipText;

        ResourceBar = new();
        ResourceBar.style.flexDirection = FlexDirection.RowReverse;
        ResourceBar.style.backgroundColor = color;
        Add(ResourceBar);

        MissingBar = new();
        _text = new();
        _text.AddToClassList(_ussCommonTextSecondary);
        ResourceBar.Add(MissingBar);
        ResourceBar.Add(_text);

        UpdateStyles(_ussContainer, _ussMain, _ussMissing, _ussBarText);
        UpdateTrackedVariables(currentIntVar, totalIntVar, totalStat);
        DisplayMissingAmount();

        RegisterCallback<DetachFromPanelEvent>(UnsubscribeFromValueChanges);
    }

    void UnsubscribeFromValueChanges(DetachFromPanelEvent evt)
    {
        if (_currentInt != null) _currentInt.OnValueChanged -= OnValueChanged;
        if (_totalInt != null) _totalInt.OnValueChanged -= OnTotalChanged;
        if (_totalStat != null) _totalStat.OnValueChanged -= _totalInt.SetValue;

    }

    void UpdateStyles(string container, string main, string missing, string text)
    {
        AddToClassList(container);
        ResourceBar.AddToClassList(main);
        MissingBar.AddToClassList(missing);
        _text.AddToClassList(text);
    }

    public void UpdateTrackedVariables(IntVariable current, IntVariable totalInt = null, Stat totalStat = null)
    {
        UnsubscribeFromValueChanges(default);

        _currentInt = current;
        _displayedAmount = _currentInt.Value;
        current.OnValueChanged += OnValueChanged;

        _totalInt = totalInt;

        if (totalStat != null)
        {
            _totalStat = totalStat;
            _totalInt = ScriptableObject.CreateInstance<IntVariable>();
            _totalInt.SetValue(totalStat.GetValue());
            _totalStat.OnValueChanged += _totalInt.SetValue;
        }

        _totalInt.OnValueChanged += OnTotalChanged;

        DisplayMissingAmount();
    }

    void OnTotalChanged(int total)
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

        if (this != null)
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
