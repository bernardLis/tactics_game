using System.Collections;

using UnityEngine;

namespace Lis
{
    public class BattleAbilityEarthSlash : BattleAbility
    {
        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            _abilityObjectParent = transform;

            transform.localPosition = new Vector3(0f, 0f, 1f); // it is where the effect spawns...
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            for (int i = 0; i < _ability.GetAmount(); i++)
            {
                BattleEarthSlash s = GetInactiveAbilityObject() as BattleEarthSlash;
                s.Execute(GetSlashPosition(i), GetSlashRotation(i));
            }
        }

        Vector3 GetSlashPosition(int i)
        {
            if (i == 0)
                return Vector3.up * 0.5f + Vector3.forward;
            if (i == 1)
                return Vector3.up * 0.5f + Vector3.forward * -1;

            return transform.position;
        }

        Quaternion GetSlashRotation(int i)
        {
            if (i == 1)
                return Quaternion.Euler(0f, 180f, 0f);
            return Quaternion.identity;
        }

    }
}
