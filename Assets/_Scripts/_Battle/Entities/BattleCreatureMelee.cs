using System.Collections;

namespace Lis
{
    public class BattleCreatureMelee : BattleCreature
    {
        BattleHitEffectPool _hitEffectPool;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _hitEffectPool = GetComponent<BattleHitEffectPool>();
        }

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, team);
            _hitEffectPool.Initialize(Creature.HitPrefab);
        }

        protected override IEnumerator PathToOpponent()
        {
            yield return base.PathToOpponent();
            Opponent.GetEngaged(this); // otherwise, creature can't catch up
        }

        protected override IEnumerator Attack()
        {
            yield return base.Attack();
            if (!IsOpponentInRange()) yield break;

            if (Opponent == null) yield break;
            if (Opponent.Collider == null) yield break;
            _hitEffectPool.GetObjectFromPool().PlayEffect(Opponent);

            yield return Opponent.GetHit(this);
        }
    }
}