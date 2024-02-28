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
            // BattleCreature.GetComponent<BattleEntityPathing>().DisableAgent();

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);
            BattleCreatureRanged bca = (BattleCreatureRanged)BattleCreature;
            Vector3 point = bca.ClosestPositionWithClearLos();
            Debug.Log($"pos: {transform.position}");
            Debug.Log($"point: {point}");
            bca.transform.position = point;

            // yield return new WaitForSeconds(0.2f);
            // BattleCreature.GetComponent<BattleEntityPathing>().EnableAgent();

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