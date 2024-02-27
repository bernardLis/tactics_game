using System.Collections;
using UnityEngine;

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

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);
            BattleCreatureRanged bca = (BattleCreatureRanged)BattleCreature;
            Vector3 point = bca.ClosestPositionWithClearLos();
            transform.position = point;

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