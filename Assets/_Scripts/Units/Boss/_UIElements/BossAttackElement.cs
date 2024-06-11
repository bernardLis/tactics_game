using Lis.Core;
using Lis.Units.Attack;
using UnityEngine.UIElements;

namespace Lis.Units.Boss
{
    public class BossAttackElement : ElementWithTooltip
    {
        private readonly AttackBoss _attack;

        public BossAttackElement(AttackBoss attack)
        {
            _attack = attack;

            style.width = 50;
            style.height = 50;
            style.backgroundImage = _attack.Icon.texture;
        }

        protected override void DisplayTooltip()
        {
            Label tooltip = new Label(_attack.name);
            _tooltip = new(this, tooltip);
            base.DisplayTooltip();
        }
    }
}