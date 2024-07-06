using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Items
{
    public class ArmorElement : ElementWithTooltip
    {
        const string _ussCommonButtonBasic = "common__button-basic";
        readonly Armor _armor;

        readonly bool _blockTooltip;

        public ArmorElement(Armor armor, bool blockTooltip = false)
        {
            AddToClassList(_ussCommonButtonBasic);

            style.alignItems = Align.Center;

            _armor = armor;
            _blockTooltip = blockTooltip;

            Label nameLabel = new(Helpers.ParseScriptableObjectName(_armor.name));
            Add(nameLabel);

            Add(new ArmorIcon(_armor, true));

            Label stat = new($"{_armor.StatType.ToString()} +{_armor.Value}");
            Add(stat);
        }

        protected override void DisplayTooltip()
        {
            if (_blockTooltip) return;
            ArmorComparisonTooltipElement tt = new(_armor);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}