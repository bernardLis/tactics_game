using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Tablets
{
    public class TabletTooltipElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "tablet-tooltip-element__";
        const string _ussMain = _ussClassName + "main";

        readonly Tablet _tablet;

        public TabletTooltipElement(Tablet tablet)
        {
            var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null) styleSheets.Add(commonStyles);
            var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TabletTooltipElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);

            _tablet = tablet;

            AddBasicInformation();
            AddCurrentEffect();
            AddNextEffect();
        }

        void AddBasicInformation()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.Center;
            Add(container);

            Label name = new(Helpers.ParseScriptableObjectName(_tablet.name));
            container.Add(name);
            ElementalElement element = new(_tablet.Nature);
            container.Add(element);

            Label level = new($"Level: {_tablet.Level.Value}");
            Add(level);

            Add(new HorizontalSpacerElement());
        }

        void AddCurrentEffect()
        {
            Label title = new("Current Effect");
            Add(title);

            Label bonusDmg = new($"{_tablet.Nature.NatureName} damage +{_tablet.Level.Value * 10}%");
            Add(bonusDmg);

            Label bonusPrimaryStat = new($"{_tablet.PrimaryStat.StatType} +{_tablet.Level.Value * _tablet.PrimaryStat.Value}");
            Add(bonusPrimaryStat);

            if (_tablet.SecondaryStat.StatType != StatType.None)
            {
                Label bonusSecondaryStat = new($"{_tablet.SecondaryStat.StatType} +{_tablet.Level.Value * _tablet.SecondaryStat.Value}");
                Add(bonusSecondaryStat);
            }

            Add(new HorizontalSpacerElement());
        }

        void AddNextEffect()
        {
            Label title = new("Next Effect");
            Add(title);

            Label bonusDmg = new($"{_tablet.Nature.NatureName} damage +{(_tablet.Level.Value + 1) * 10}%");
            Add(bonusDmg);

            Label bonusPrimaryStat = new($"{_tablet.PrimaryStat.StatType} +{(_tablet.Level.Value + 1) * _tablet.PrimaryStat.Value}");
            Add(bonusPrimaryStat);

            if (_tablet.SecondaryStat.StatType != StatType.None)
            {
                Label bonusSecondaryStat = new($"{_tablet.SecondaryStat.StatType} +{(_tablet.Level.Value + 1) * _tablet.SecondaryStat.Value}");
                Add(bonusSecondaryStat);
            }

        }
    }
}
