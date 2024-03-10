
using UnityEngine.UIElements;

namespace Lis.Units.Boss
{
    public class BossScreen : UnitMovementScreen
    {
        readonly Boss _boss;
        public BossScreen(Boss boss) : base(boss)
        {
            _boss = boss;
        }

        public override void Initialize()
        {
            base.Initialize();
            AddBossAttacks();
        }

        void AddBossAttacks()
        {
            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;

            MainCardContainer.Add(container);

            foreach (var attack in _boss.Attacks)
            {
                BossAttackElement attackElement = new(attack);
                container.Add(attackElement);
            }
        }

    }
}
