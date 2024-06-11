using UnityEngine.UIElements;

namespace Lis.Units.Creature
{
    public class CreatureScreen : UnitScreen
    {
        private readonly Creature _creature;

        public CreatureScreen(Creature creature)
            : base(creature)
        {
            _creature = creature;
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
            Label upgradeTier = new($"Tier: {_creature.UpgradeTier}");
            OtherContainer.Add(upgradeTier);
        }

        private void AddAbility()
        {
            if (_creature.SpecialAttack == null) return;
            // HERE: attack element OtherContainer.Insert(0, new Element(Creature.Ability, isLocked: !Creature.IsAbilityUnlocked()));
        }
    }
}