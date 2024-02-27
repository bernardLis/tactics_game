using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBomb : BattleCreatureRanged
    {
        [SerializeField] float _explosionRadius = 5f;
        [SerializeField] GameObject _explosionEffect;
        //
        // public override IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
        // {
        //     yield return ManageCreatureAbility();
        //     Invoke(nameof(CleanUp), 2f);
        //     yield return base.Die(attacker, hasLoot);
        // }
        //
        // protected override IEnumerator CreatureAbility()
        // {
        //     yield return base.CreatureAbility();
        //
        //     _explosionEffect.SetActive(true);
        //     Collider[] colliders = new Collider[10];
        //     Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, colliders);
        //     foreach (Collider c in colliders)
        //     {
        //         if (c == null) continue;
        //         if (!c.TryGetComponent(out BattleEntity entity)) continue;
        //         if (entity.Team == Team) continue; // splash damage is player friendly
        //         if (entity.IsDead) continue;
        //
        //         StartCoroutine(entity.GetHit(this, 50));
        //     }
        // }
        //
        // void CleanUp()
        // {
        //     _explosionEffect.SetActive(false);
        // }
    }
}