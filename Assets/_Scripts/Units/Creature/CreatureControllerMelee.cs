using System.Collections;

namespace Lis.Units.Creature
{
    public class CreatureControllerMelee : CreatureController
    {
        HitEffectPoolManager _hitEffectPoolManager;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _hitEffectPoolManager = GetComponent<HitEffectPoolManager>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _hitEffectPoolManager.Initialize(Creature.HitPrefab);
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
            _hitEffectPoolManager.GetObjectFromPool().PlayEffect(Opponent);

            yield return Opponent.GetHit(this);
        }
    }
}