using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleCreatureMelee : BattleCreature
{
    GameObject _hitInstance;

    protected override IEnumerator PathToOpponent()
    {
        yield return base.PathToOpponent();
        Opponent.Engage(this); // otherwise, creature can't catch up
    }

    protected override IEnumerator Attack()
    { 
        yield return base.Attack();

        Quaternion q = Quaternion.Euler(0, -90, 0);
        _hitInstance = Instantiate(Creature.HitPrefab, Opponent.Collider.bounds.center, q);
        _hitInstance.transform.parent = Opponent.transform;

        yield return Opponent.GetHit(this);
        Invoke(nameof(DestroyHitInstance), 2f);
    }

    void DestroyHitInstance()
    {
        Destroy(_hitInstance);
    }
}
