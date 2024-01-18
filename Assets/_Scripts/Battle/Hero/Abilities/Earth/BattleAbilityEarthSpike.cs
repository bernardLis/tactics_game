using System.Collections;

using UnityEngine;

namespace Lis
{
    public class BattleAbilityEarthSpike : BattleAbility
    {

        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new Vector3(0f, 0f, 4f); // it is where the effect spawns...
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            BattleEarthSpike spike = GetInactiveAbilityObject() as BattleEarthSpike;
            spike.Execute(transform.position, transform.rotation);
        }

    }
}
