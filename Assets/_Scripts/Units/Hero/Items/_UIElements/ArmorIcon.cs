using Lis.Core;

namespace Lis.Units.Hero.Items
{
    public class ArmorIcon : ElementWithTooltip
    {
        readonly Armor _armor;
        readonly bool _blockTooltip;

        public ArmorIcon(Armor armor, bool blockTooltip = false)
        {
            _armor = armor;
            _blockTooltip = blockTooltip;

            style.width = 100;
            style.height = 100;
            style.backgroundImage = new(_armor.Icon);
        }


        protected override void DisplayTooltip()
        {
            if (_blockTooltip) return;
            ArmorElement tt = new(_armor);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}