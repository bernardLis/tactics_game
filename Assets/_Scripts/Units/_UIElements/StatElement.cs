using System.Globalization;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class StatElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "stat-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussValue = _ussClassName + "value";

        readonly Stat _stat;

        Label _icon;

        string _tooltipText;
        Label _value;

        public StatElement(Stat stat)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.StatElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            AddToClassList(_ussMain);
            _stat = stat;
            BaseStatVisual();
            _stat.OnValueChanged += UpdateValue;
        }

        void BaseStatVisual()
        {
            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = new(_stat.Icon);
            Add(_icon);

            _value = new();
            _value.AddToClassList(_ussValue);
            _value.AddToClassList(_ussCommonTextPrimary);
            Add(_value);
            UpdateValue(_stat.GetValue());

            string description = _stat.Description;
            if (description.Length == 0)
                description = _stat.StatType.ToString();
            _tooltipText = description;
        }

        void UpdateValue(float value)
        {
            _value.text = value == 0 ? "0" : value.ToString("#.#", CultureInfo.InvariantCulture);
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, new Label(_tooltipText));
            base.DisplayTooltip();
        }
    }
}