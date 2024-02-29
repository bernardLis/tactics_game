using Lis.Units.Creature.Ability;
using UnityEngine.UIElements;

namespace Lis.Units.Creature
{
    public class CreatureScreen : UnitFightScreen
    {
        public Creature Creature;

        public CreatureScreen(Creature creature)
            : base(creature)
        {
            Creature = creature;
        }

        public override void Initialize()
        {
            base.Initialize();
            UnitIcon.PlayAnimationAlways();

            AddAbility();
        }

        protected override void AddOtherBasicInfo()
        {
            base.AddOtherBasicInfo();
            Label upgradeTier = new($"Tier: {Creature.UpgradeTier}");
            OtherContainer.Add(upgradeTier);
        }

        void AddAbility()
        {
            if (Creature.Ability == null) return;
            OtherContainer.Insert(0, new Element(Creature.Ability, isLocked: !Creature.IsAbilityUnlocked()));
        }
    }
}
