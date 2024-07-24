using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Enemy;
using Lis.Units.Pawn;
using Lis.Units.Peasant;

namespace Lis.Arena
{
    public class UnitCardFactory : Singleton<UnitCardFactory>
    {
        public UnitCard CreateUnitCard(Unit unit)
        {
            if (unit is Enemy enemy) return new EnemyCard(enemy);
            if (unit is Peasant peasant) return new PeasantCard(peasant);
            if (unit is Pawn pawn) return new PawnCard(pawn);

            return new(unit);
        }
    }
}