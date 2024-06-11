using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Enemy
{
    public class EnemyScreen : UnitScreen
    {
        private Enemy _enemy;

        public EnemyScreen(Unit unit) : base(unit)
        {
        }

        protected override void AddLevel()
        {
            _enemy = (Enemy)Unit;
            Label l = new(
                $"<b>{Helpers.ParseScriptableObjectName(Unit.name)} Scariness {_enemy.ScarinessRank}<b>");
            BasicInfoContainer.Add(l);
        }
    }
}