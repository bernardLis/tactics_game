using System.Collections;
using Lis.Battle.Fight;
using Lis.Units.Creature;
using Lis.Units.Projectile;
using Shapes;
using UnityEngine;

namespace Lis.Units.Minion
{
    public class RangedMinionController : CreatureControllerRanged
    {
        RangedOpponentManager _rangedOpponentManager;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            if (Gfx != null) Gfx.SetActive(true);
            Gfx.GetComponentInChildren<Sphere>().Color = unit.Nature.Color.Primary;

            if (_rangedOpponentManager == null)
                _rangedOpponentManager = BattleManager.GetComponent<RangedOpponentManager>();

            // pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;
        }

        protected override IEnumerator Attack()
        {
            yield return base.Attack();

            OpponentProjectileController p = _rangedOpponentManager.GetProjectileFromPool(Unit.Nature.NatureName);
            Vector3 pos = transform.position;
            p.transform.position = pos;
            p.Initialize(1);

            Vector3 dir = (Opponent.transform.position - pos).normalized;
            dir.y = 0;
            p.Shoot(this, dir, 15, Mathf.FloorToInt(Creature.Power.GetValue()));
            yield return null;
        }


        public override IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}