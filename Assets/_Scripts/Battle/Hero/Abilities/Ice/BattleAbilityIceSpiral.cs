using System.Collections;

using UnityEngine;

namespace Lis
{
    public class BattleAbilityIceSpiral : BattleAbility
    {
        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = Vector3.zero;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            BattleIceSpiral spiral = GetInactiveAbilityObject() as BattleIceSpiral;
            spiral.Execute(transform.position, Quaternion.identity);
        }
    }
}
