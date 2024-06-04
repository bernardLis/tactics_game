using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Enemy;
using Lis.Units.Hero;
using Lis.Units.Pawn;

namespace Lis.Battle
{
    public class UnitScreenFactory : Singleton<UnitScreenFactory>
    {
        public UnitScreen CreateUnitScreen(Unit unit)
        {
            if (unit is Enemy enemy) return new EnemyScreen(enemy);
            if (unit is Creature creature) return new CreatureScreen(creature);
            if (unit is Pawn pawn) return new PawnScreen(pawn);
            if (unit is Hero hero) return new HeroScreen(hero);
            if (unit is Boss boss) return new BossScreen(boss);

            return new(unit);
        }
    }
}