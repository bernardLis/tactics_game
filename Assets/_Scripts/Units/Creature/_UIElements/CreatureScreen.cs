using UnityEngine.UIElements;

namespace Lis.Units.Creature
{
    public class CreatureScreen : UnitScreen
    {
        readonly Creature _creature;

        public CreatureScreen(Creature creature)
            : base(creature)
        {
            _creature = creature;
        }

        protected override void AddOtherBasicInfo()
        {
            base.AddOtherBasicInfo();
            Label upgradeTier = new($"Tier: {_creature.UpgradeTier}");
            OtherContainer.Add(upgradeTier);
        }
    }
}