using Lis.Core;
using Lis.Units.Boss.Attack;
using UnityEngine.UIElements;

namespace Lis.Units.Boss
{
    public class BossAttackElement : ElementWithTooltip
    {
        readonly Units.Attack.AttackBoss _attack;
        public BossAttackElement(Units.Attack.AttackBoss attack)
        {
            _attack = attack;

            style.width = 50;
            style.height = 50;
            style.backgroundImage = _attack.Icon.texture;
        }

        protected override void DisplayTooltip()
        {
            var tooltip = new Label(_attack.name);
            _tooltip = new(this, tooltip);
            base.DisplayTooltip();
        }

    }
}
