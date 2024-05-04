using Lis.Battle.Fight;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Creature
{
    public class CreatureControllerRanged : CreatureController
    {
        [FormerlySerializedAs("_projectileSpawnPoint")] [SerializeField]

        RangedOpponentManager _rangedOpponentManager;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (Creature.Projectile == null) return;

            if (team == 1) InitializeHostileCreature();
        }

        protected override void InitializeHostileCreature()
        {
            base.InitializeHostileCreature();
            _rangedOpponentManager = BattleManager.GetComponent<RangedOpponentManager>();
        }


    }
}