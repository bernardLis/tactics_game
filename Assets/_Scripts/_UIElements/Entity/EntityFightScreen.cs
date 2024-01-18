
using UnityEngine.UIElements;

namespace Lis
{
    public class EntityFightScreen : EntityMovementScreen
    {
        readonly EntityFight _entityFight;
        public EntityFightScreen(EntityFight entity) : base(entity)
        {
            _entityFight = entity;
        }

        public override void Initialize()
        {
            base.Initialize();
            AddBattleData();
        }

        protected override void AddStats()
        {
            base.AddStats();
            StatElement power = new(_entityFight.Power);
            _statsContainer.Add(power);
            StatElement attackRange = new(_entityFight.AttackRange);
            _statsContainer.Add(attackRange);
            StatElement attackCooldown = new(_entityFight.AttackCooldown);
            _statsContainer.Add(attackCooldown);
        }

        void AddBattleData()
        {
            Label killCount = new($"Kill Count: {_entityFight.TotalKillCount}");
            Label damageDealt = new($"Damage Dealt: {_entityFight.TotalDamageDealt}");
            Label damageTaken = new($"Damage Taken: {_entityFight.TotalDamageTaken}");

            _otherContainer.Add(killCount);
            _otherContainer.Add(damageDealt);
            _otherContainer.Add(damageTaken);
        }

    }
}
