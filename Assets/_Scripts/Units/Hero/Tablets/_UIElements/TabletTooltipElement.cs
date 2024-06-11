using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Tablets
{
    public class TabletTooltipElement : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        private const string _ussClassName = "tablet-tooltip-element__";
        private const string _ussMain = _ussClassName + "main";

        private readonly VisualElement _effectContainer;

        private readonly Tablet _tablet;

        public TabletTooltipElement(Tablet tablet)
        {
            StyleSheet commonStyles = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null) styleSheets.Add(commonStyles);
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TabletTooltipElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);

            _tablet = tablet;

            AddBasicInformation();

            _effectContainer = new();
            _effectContainer.style.flexDirection = FlexDirection.Row;
            _effectContainer.style.justifyContent = Justify.Center;
            Add(_effectContainer);

            AddCurrentEffect();
            AddNextEffect();
        }

        private void AddBasicInformation()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.Center;
            Add(container);

            Label n = new(Helpers.ParseScriptableObjectName(_tablet.name));
            container.Add(n);
            NatureElement element = new(_tablet.Nature);
            container.Add(element);

            Label level = new($"Level: {_tablet.Level.Value}");
            Add(level);

            Add(new HorizontalSpacerElement());
        }

        private void AddCurrentEffect()
        {
            VisualElement container = new();

            Label title = new("Current Effect");
            container.Add(title);

            Label bonusDmg = new($"{_tablet.Nature.NatureName} damage +{_tablet.Level.Value * 10}%");
            container.Add(bonusDmg);

            Label bonusPrimaryStat =
                new($"{_tablet.PrimaryStat.StatType} +{_tablet.Level.Value * _tablet.PrimaryStat.Value}");
            container.Add(bonusPrimaryStat);

            if (_tablet.SecondaryStat.StatType != StatType.None)
            {
                Label bonusSecondaryStat =
                    new($"{_tablet.SecondaryStat.StatType} +{_tablet.Level.Value * _tablet.SecondaryStat.Value}");
                container.Add(bonusSecondaryStat);
            }

            _effectContainer.Add(container);
        }

        private void AddNextEffect()
        {
            VisualElement container = new();
            container.style.borderLeftWidth = 3;
            container.style.borderLeftColor = Color.white;

            Label title = new("Next Effect");
            container.Add(title);

            Label bonusDmg = new($"{_tablet.Nature.NatureName} damage +{(_tablet.Level.Value + 1) * 10}%");
            container.Add(bonusDmg);

            Label bonusPrimaryStat =
                new($"{_tablet.PrimaryStat.StatType} +{(_tablet.Level.Value + 1) * _tablet.PrimaryStat.Value}");
            container.Add(bonusPrimaryStat);

            if (_tablet.SecondaryStat.StatType != StatType.None)
            {
                Label bonusSecondaryStat =
                    new($"{_tablet.SecondaryStat.StatType} +{(_tablet.Level.Value + 1) * _tablet.SecondaryStat.Value}");
                container.Add(bonusSecondaryStat);
            }

            _effectContainer.Add(container);
        }
    }
}