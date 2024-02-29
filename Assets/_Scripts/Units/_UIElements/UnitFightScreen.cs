
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitFightScreen : UnitMovementScreen
    {
        readonly UnitFight _unitFight;
        public UnitFightScreen(UnitFight unit) : base(unit)
        {
            _unitFight = unit;
        }

        public override void Initialize()
        {
            base.Initialize();
            AddBattleData();
        }

        protected override void AddStats()
        {
            base.AddStats();
            StatElement power = new(_unitFight.Power);
            StatsContainer.Add(power);
            StatElement attackRange = new(_unitFight.AttackRange);
            StatsContainer.Add(attackRange);
            StatElement attackCooldown = new(_unitFight.AttackCooldown);
            StatsContainer.Add(attackCooldown);
        }

        void AddBattleData()
        {
            Label killCount = new($"Kill Count: {_unitFight.TotalKillCount}");
            Label damageDealt = new($"Damage Dealt: {_unitFight.TotalDamageDealt}");
            Label damageTaken = new($"Damage Taken: {_unitFight.TotalDamageTaken}");

            OtherContainer.Add(killCount);
            OtherContainer.Add(damageDealt);
            OtherContainer.Add(damageTaken);
        }

    }
}
