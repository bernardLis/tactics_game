using System.Collections;

using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleDragonSpark : BattleCreatureRanged
    {
        [SerializeField] GameObject _abilityProjectile;

        protected override IEnumerator Attack()
        {
            yield return ManageCreatureAbility();
            yield return base.Attack();
        }

        protected override IEnumerator CreatureAbility()
        {
            if (!IsOpponentInRange()) yield break;

            yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f).WaitForCompletion();
            yield return base.CreatureAbility();
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

            GameObject projectileInstance = Instantiate(_abilityProjectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
            projectileInstance.transform.parent = Gfx.transform;
            BattleProjectile p = projectileInstance.GetComponent<BattleProjectile>();
            p.Initialize(Team);
            Vector3 dir = (Opponent.transform.position - transform.position).normalized;
            p.Shoot(Creature, dir);
        }
    }
}
