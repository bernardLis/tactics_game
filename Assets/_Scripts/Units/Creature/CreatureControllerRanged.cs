using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core.Utilities;
using Lis.Units.Projectile;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Creature
{
    public class CreatureControllerRanged : CreatureController
    {
        [FormerlySerializedAs("_projectileSpawnPoint")] [SerializeField]
        public GameObject ProjectileSpawnPoint;

        ProjectilePoolManager _projectilePoolManager;

        RangedOpponentManager _rangedOpponentManager;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _projectilePoolManager = GetComponent<ProjectilePoolManager>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (Creature.Projectile == null) return;
            _projectilePoolManager.Initialize(Creature.Projectile);

            if (team == 1) InitializeHostileCreature();
        }

        protected override void InitializeHostileCreature()
        {
            base.InitializeHostileCreature();
            _rangedOpponentManager = BattleManager.GetComponent<RangedOpponentManager>();
        }


        // protected override IEnumerator AttackCoroutine()
        // {
        //     yield return base.AttackCoroutine();
        //
        //     ProjectileController projectileController = Team == 0
        //         ? _projectilePoolManager.GetObjectFromPool()
        //         : _rangedOpponentManager.GetProjectileFromPool(Unit.Nature.NatureName);
        //     projectileController.Initialize(Team);
        //     projectileController.transform.position = ProjectileSpawnPoint.transform.position;
        //     if (Opponent == null) yield break;
        //     Vector3 dir = (Opponent.transform.position - transform.position).normalized;
        //     dir.y = 0;
        //
        //     if (Team == 0)
        //         projectileController.Shoot(this, dir);
        //     if (Team == 1)
        //     {
        //         OpponentProjectileController p = (OpponentProjectileController)projectileController;
        //         p.Shoot(this, dir, 15, Mathf.FloorToInt(Creature.Power.GetValue()));
        //     }
        // }
    }
}