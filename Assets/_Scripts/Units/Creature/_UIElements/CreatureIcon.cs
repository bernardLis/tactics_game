using Lis.Core;

namespace Lis.Units.Creature
{
    public class CreatureIcon : UnitIcon
    {
        OverlayTimerElement _overlayTimer;

        // used only in troops element for now, maybe should have different name?
        public CreatureIcon(Creature creature, bool blockClick = false) : base(creature, blockClick)
        {
            style.width = 50;
            style.height = 50;

            Icon.style.width = 45;
            Icon.style.height = 45;
        }

        public void SetBigIcon()
        {
            style.width = 150;
            style.height = 150;

            Icon.style.width = 140;
            Icon.style.height = 140;
        }
    }
}