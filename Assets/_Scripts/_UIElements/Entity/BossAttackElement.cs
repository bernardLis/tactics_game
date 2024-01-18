

using UnityEngine.UIElements;

namespace Lis
{
    public class BossAttackElement : ElementWithTooltip
    {
        BossAttack _bossAttack;
        public BossAttackElement(BossAttack bossAttack)
        {
            _bossAttack = bossAttack;

            style.width = 50;
            style.height = 50;
            style.backgroundImage = _bossAttack.Icon.texture;
        }

        protected override void DisplayTooltip()
        {
            var tooltip = new Label(_bossAttack.name);
            _tooltip = new(this, tooltip);
            base.DisplayTooltip();
        }

    }
}
