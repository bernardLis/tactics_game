using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class FightIcon : ElementWithTooltip
    {
        public FightIcon()
        {
            AddToClassList("common__fight-icon");
        }

        protected override void DisplayTooltip()
        {
            Label tt = new("Fight");
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}