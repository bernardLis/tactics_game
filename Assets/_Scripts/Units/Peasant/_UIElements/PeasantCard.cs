using Lis.Arena;
using Lis.Core;

namespace Lis.Units.Peasant
{
    public class PeasantCard : UnitCard
    {
        const string _ussCommonButton = "common__button";

        readonly Peasant _peasant;

        public PeasantCard(Peasant peasant) : base(peasant)
        {
            _peasant = peasant;
            if (FightManager.IsFightActive) return;
            if (_peasant.CurrentHealth.Value <= 0) return;
        }
    }
}