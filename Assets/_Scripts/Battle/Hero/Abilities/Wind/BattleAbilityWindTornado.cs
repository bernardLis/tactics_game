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

            for (int i = 0; i < _ability.GetAmount(); i++)
            {
                Vector3 pos = new(transform.position.x, 0, transform.position.z);
                Quaternion q = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                BattleWindTornado tornado = GetInactiveAbilityObject() as BattleWindTornado;
                tornado.Execute(pos, q);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
