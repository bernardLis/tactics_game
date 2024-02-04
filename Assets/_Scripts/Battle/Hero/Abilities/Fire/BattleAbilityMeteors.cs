using System.Collections;

using UnityEngine;

namespace Lis
{
    public class BattleAbilityMeteors : BattleAbility
    {
        public override void Initialize(Ability ability, bool startAbility)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new Vector3(-0.5f, 1f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                // random position within circle radius
                Vector3 pos = BattleAreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position, Random.Range(10, 20));
                BattleMeteors meteor = GetInactiveAbilityObject() as BattleMeteors;
                meteor.Execute(pos, Quaternion.identity);
            }
        }
    }
}
