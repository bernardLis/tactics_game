using System;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ResourceBarElement : ElementWithTooltip
    {
        const string _ussCommonTextSecondary = "common__text-secondary";

        const string _ussClassName = "resource-bar";
        const string _ussContainer = _ussClassName + "__container";
        const string _ussMain = _ussClassName + "__main";
        const string _ussMissing = _ussClassName + "__missing";
        const string _ussBarText = _ussClassName + "__bar-text";
        readonly Label _text;

        readonly string _tooltipText;

        readonly int _valueChangeDelay;
        public readonly VisualElement MissingBar;

        public readonly VisualElement ResourceBar;

        IVisualElementScheduledItem _animation;
        FloatVariable _current;

        float _displayedAmount;
        FloatVariable _total;
        Stat _totalStat;

        public ResourceBarElement(Color color, string tooltipText,
            FloatVariable currentFloatVar,
            FloatVariable totalFloatVar = null, Stat totalStat = null,
            int valueChangeDelayMs = 200)
        {
            GameManager gameManager = GameManager.Instance;
            StyleSheet ss = gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ResourceBarStyles);
            if (ss != null) styleSheets.Add(ss);

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
            UpdateTrackedVariables(currentFloatVar, totalFloatVar, totalStat);
            DisplayMissingAmount();

            RegisterCallback<DetachFromPanelEvent>(UnsubscribeFromValueChanges);
        }

        public event Action OnAnimationFinished;

        void UnsubscribeFromValueChanges(DetachFromPanelEvent evt)
        {
            if (_current != null) _current.OnValueChanged -= OnValueChanged;
            if (_total != null) _total.OnValueChanged -= OnTotalChanged;
            if (_totalStat != null) _totalStat.OnValueChanged -= _total.SetValue;
        }

        void UpdateStyles(string container, string main, string missing, string text)
        {
            AddToClassList(container);
            ResourceBar.AddToClassList(main);
            MissingBar.AddToClassList(missing);
            _text.AddToClassList(text);
        }

        public void UpdateTrackedVariables(FloatVariable current, FloatVariable totalFloat = null,
            Stat totalStat = null)
        {
            UnsubscribeFromValueChanges(default);

            _current = current;
            _displayedAmount = _current.Value;
            current.OnValueChanged += OnValueChanged;

            _total = totalFloat;

            if (totalStat != null)
            {
                _totalStat = totalStat;
                _total = ScriptableObject.CreateInstance<FloatVariable>();
                _total.SetValue(totalStat.GetValue());
                _totalStat.OnValueChanged += _total.SetValue;
            }

            if (_total == null) return;

            _total.OnValueChanged += OnTotalChanged;
            DisplayMissingAmount();
        }

        void OnTotalChanged(float _)
        {
            DisplayMissingAmount();
        }

        void DisplayMissingAmount()
        {
            if (_total == null) return;

            MissingBar.style.display = DisplayStyle.Flex;

            float missingPercent = _displayedAmount / _total.Value;
            missingPercent = Mathf.Clamp(missingPercent, 0, 1);

            MissingBar.style.width = Length.Percent((1 - missingPercent) * 100);

            SetText($"{Mathf.FloorToInt(_displayedAmount)}/{Mathf.FloorToInt(_total.Value)}");
        }

        void SetText(string newText)
        {
            _text.text = newText;
        }

        public void ChangeValueNoAnimation(int value)
        {
            _displayedAmount = value;
            DisplayMissingAmount();
            _current.OnValueChanged -= OnValueChanged;
            _current.SetValue(value);
            _current.OnValueChanged += OnValueChanged;
        }

        void OnValueChanged(float newValue)
        {
            float change = Mathf.Abs(newValue - _current.PreviousValue);

            if (change == 0) return;

            if (_animation != null)
            {
                _animation.Pause();
                _displayedAmount = _current.PreviousValue;
                DisplayMissingAmount();
            }

            int delay = Mathf.FloorToInt(_valueChangeDelay / change); // do it in 1second

            if (newValue - _current.PreviousValue < 0)
                _animation = schedule.Execute(HandleDecrease).Every(delay);
            else
                _animation = schedule.Execute(HandleIncrease).Every(delay);
        }

        void HandleDecrease()
        {
            if (_current.Value >= _displayedAmount)
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
            if (_current.Value <= _displayedAmount)
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

        public void HideText()
        {
            _text.style.display = DisplayStyle.None;
        }
    }
}