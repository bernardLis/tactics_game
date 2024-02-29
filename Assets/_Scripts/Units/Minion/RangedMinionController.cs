using System.Collections;
using Lis.Units.Creature;
using Lis.Units.Projectile;
using Shapes;
using UnityEngine;

namespace Lis.Units.Minion
{
    public class RangedMinionController : CreatureControllerRanged
    {
        BattleRangedOpponentManager _battleRangedOpponentManager;

        public override void InitializeEntity(Unit unit, int team)
        {
            base.InitializeEntity(unit, team);

            if (Gfx != null) Gfx.SetActive(true);
            Gfx.GetComponentInChildren<Sphere>().Color = unit.Element.Color.Primary;

            if (_battleRangedOpponentManager == null)
                _battleRangedOpponentManager = BattleManager.GetComponent<BattleRangedOpponentManager>();

            // pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;
        }

        protected override IEnumerator Attack()
        {
            yield return base.Attack();

            OpponentProjectileController p = _battleRangedOpponentManager.GetProjectileFromPool(Unit.Element.ElementName);
            Vector3 pos = transform.position;
            p.transform.position = pos;
            p.Initialize(1);

            Vector3 dir = (Opponent.transform.position - pos).normalized;
            dir.y = 0;
            p.Shoot(this, dir, 15, Mathf.FloorToInt(Creature.Power.GetValue()));
            yield return null;
        }


        public override IEnumerator Die(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}