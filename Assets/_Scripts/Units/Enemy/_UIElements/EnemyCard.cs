using Lis.Battle;
using UnityEngine;

namespace Lis.Units.Enemy
{
    public class EnemyCard : UnitCard
    {
        Enemy _enemy;

        public EnemyCard(Unit unit) : base(unit)
        {
            Debug.Log("EnemyCard ctor");
        }

        protected override void HandleLevelLabel()
        {
            base.HandleLevelLabel();
            _enemy = (Enemy)Unit;

            Debug.Log("EnemyCard HandleLevelLabel");

            LevelLabel.text = $"Scariness rank: {_enemy.ScarinessRank}";
        }
    }
}