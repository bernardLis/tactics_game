using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleAbilityWindTornado : BattleAbility
    {
        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new Vector3(0.5f, 1f, 0.5f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                Vector3 currentPosition = transform.position;
                Vector3 pos = new(currentPosition.x, 0, currentPosition.z);
                Quaternion look = Quaternion.LookRotation(GetRandomEnemyDirection(),
                    Vector3.up);

                BattleWindTornado tornado = GetInactiveAbilityObject() as BattleWindTornado;
                if (tornado != null) tornado.Execute(pos, look);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}