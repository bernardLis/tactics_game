using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class EarthSlashController : Controller
    {
        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            AbilityObjectParent = transform;

            transform.localPosition = new(0f, 0f, 1f); // it is where the effect spawns...
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            for (int i = 0; i < Ability.GetAmount(); i++)
            {

                EarthSlashObjectController s = GetInactiveAbilityObject() as EarthSlashObjectController;
                if (s != null) s.Execute(GetSlashPosition(i), GetSlashRotation(i));
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