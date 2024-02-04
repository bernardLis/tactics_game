using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleAbilityWaterTornado : BattleAbility
    {
        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new(0.5f, 1f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                Vector3 pos = BattleManager.GetRandomEnemyPosition();
                BattleWaterTornado tornado = GetInactiveAbilityObject() as BattleWaterTornado;
                if (tornado != null) tornado.Execute(pos, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}