using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityFireArrow : BattleCreatureAbility
    {
        [SerializeField] GameObject _abilityProjectile;
        BattleProjectile _projectile;

        public override void Initialize(BattleCreature battleCreature)
        {
            base.Initialize(battleCreature);
            _projectile = Instantiate(_abilityProjectile, BattleManager.EntityHolder).GetComponent<BattleProjectile>();
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (!BattleCreature.IsOpponentInRange()) yield break;

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(CreatureAbility.Sound, transform.position);

            Vector3 oppPos = BattleCreature.Opponent.transform.position;
            yield return transform.DODynamicLookAt(oppPos, 0.2f).WaitForCompletion();

            BattleCreatureRanged bcr = (BattleCreatureRanged)BattleCreature;
            _projectile.transform.position = bcr.ProjectileSpawnPoint.transform.position;
            _projectile.Initialize(bcr.Team);
            Vector3 dir = (oppPos - transform.position).normalized;
            _projectile.Shoot(bcr, dir);
            yield return base.ExecuteAbilityCoroutine();
        }
    }
}