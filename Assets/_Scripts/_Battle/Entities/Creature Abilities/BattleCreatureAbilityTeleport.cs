using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Lis
{
    public class BattleCreatureAbilityTeleport : BattleCreatureAbility
    {
        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        public override void Initialize(BattleCreature battleCreature)
        {
            base.Initialize(battleCreature);
            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity, BattleManager.EntityHolder);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(CreatureAbility.Sound, transform.position);

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);
            BattleCreatureRanged bca = (BattleCreatureRanged)BattleCreature;
            Vector3 point = bca.ClosestPositionWithClearLos();
            bca.transform.position = point;

            Invoke(nameof(CleanUp), 2f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            if (_effectInstance != null)
                _effectInstance.SetActive(false);
        }
    }
}