


namespace Lis
{
    public class CreatureCard : EntityCard
    {
        protected Creature _creature;

        public CreatureCard(Creature creature) : base(creature)
        {
            _creature = creature;

            PopulateCard();

            if (_creature.CreatureAbility != null)
                _topRightContainer.Add(new CreatureAbilityElement(_creature.CreatureAbility));
        }
    }
}
