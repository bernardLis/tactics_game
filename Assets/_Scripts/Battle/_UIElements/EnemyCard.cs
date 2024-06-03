using Lis.Units;
using Lis.Units.Enemy;

namespace Lis.Battle
{
    public class EnemyCard : UnitCard
    {
        readonly Enemy _enemy;

        public EnemyCard(Unit unit) : base(unit)
        {
            _enemy = (Enemy)unit;
        }

        protected override void HandleLevelLabel()
        {
            base.HandleLevelLabel();
            LevelLabel.text = $"Scariness rank: {_enemy.ScarinessRank}";
        }
    }
}