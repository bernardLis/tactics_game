using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityWhirlwind: BattleCreatureAbility
    {
        readonly float _explosionRadius = 5f;
        [SerializeField] GameObject _effect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            _effect.SetActive(true);
            Collider[] colliders = new Collider[25];
            Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, colliders);
            foreach (Collider c in colliders)
            {
                if (c == null) continue;
                if (!c.TryGetComponent(out BattleEntity entity)) continue;
                if (entity.Team == Creature.Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(BattleCreature, 50));
            }

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}