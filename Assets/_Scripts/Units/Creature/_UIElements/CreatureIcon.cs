using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Creature
{
    public class CreatureIcon : UnitIcon
    {
        OverlayTimerElement _overlayTimer;

        // used only in troops element for now, maybe should have different name?
        public CreatureIcon(Creature creature, bool blockClick = false) : base(creature, blockClick)
        {
            UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);

            style.width = 50;
            style.height = 50;

            IconContainer.style.width = 45;
            IconContainer.style.height = 45;

            Frame.style.width = 50;
            Frame.style.height = 50;
        }

        public void SetBigIcon()
        {
            style.width = 150;
            style.height = 150;

            IconContainer.style.width = 140;
            IconContainer.style.height = 140;

            Frame.style.width = 150;
            Frame.style.height = 150;
        }
    }
}