using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

namespace Lis
{
    public class BattleRangedOpponent : BattleCreatureRanged
    {
        BattleRangedOpponentManager _battleRangedOpponentManager;

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, team);

            if (Gfx != null) Gfx.SetActive(true);
            Gfx.GetComponentInChildren<Sphere>().Color = entity.Element.Color.Primary;

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

            BattleProjectileOpponent p = _battleRangedOpponentManager
                .GetProjectileFromPool(Entity.Element.ElementName);
            Vector3 pos = transform.position;
            p.transform.position = pos;
            p.Initialize(1);

            Vector3 dir = (Opponent.transform.position - pos).normalized;
            dir.y = 0; // TODO: creatures miss small minions entirely
            p.Shoot(this, dir, 15, Mathf.FloorToInt(Creature.Power.GetValue()));
            yield return null;
        }


        public override IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }
    }
}