using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureMelee : BattleCreature
    {
        GameObject _hitInstance;

        protected override IEnumerator PathToOpponent()
        {
            yield return base.PathToOpponent();
            Opponent.GetEngaged(this); // otherwise, creature can't catch up
        }

        protected override IEnumerator Attack()
        {
            yield return base.Attack();

            Quaternion q = Quaternion.Euler(0, -90, 0);
            _hitInstance = Instantiate(Creature.HitPrefab, Opponent.Collider.bounds.center, q);
            _hitInstance.transform.SetParent(BattleManager.EntityHolder);
        
            yield return Opponent.GetHit(Creature);
            Invoke(nameof(DestroyHitInstance), 2f);
        }

        void DestroyHitInstance()
        {
            Destroy(_hitInstance);
        }
    }
}
