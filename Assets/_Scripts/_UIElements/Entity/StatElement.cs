using System.Globalization;
using UnityEngine.UIElements;

namespace Lis
{
    public class StatElement : ElementWithTooltip
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "stat-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussValue = _ussClassName + "value";

        readonly GameManager _gameManager;

        Label _icon;
        Label _value;

        readonly Stat _stat;

        string _tooltipText;

        public StatElement(Stat stat)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
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
            _icon.style.backgroundImage = new StyleBackground(_stat.Icon);
            Add(_icon);

            _value = new();
            _value.AddToClassList(_ussValue);
            _value.AddToClassList(_ussCommonTextPrimary);
            _value.text = _stat.GetValue().ToString(CultureInfo.InvariantCulture);
            Add(_value);

            string description = _stat.Description;
            if (description.Length == 0)
                description = _stat.StatType.ToString();
            _tooltipText = description;
        }

        void UpdateValue(float value)
        {
            _value.text = value.ToString(CultureInfo.InvariantCulture);
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, new Label(_tooltipText));
            base.DisplayTooltip();
        }
    }
}