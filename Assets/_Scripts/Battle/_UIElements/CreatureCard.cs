using Lis.Units.Creature;
using Lis.Units.Creature.Ability;

namespace Lis.Battle
{
    public class CreatureCard : UnitCard
    {
        protected Creature _creature;

        public CreatureCard(Creature creature) : base(creature)
        {
            _creature = creature;

            PopulateCard();

            if (_creature.Ability != null)
                _topRightContainer.Add(new Element(_creature.Ability));
        }
    }
}
