using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class IceSpiralController : Controller
    {
        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = Vector3.zero;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            IceSpiralObjectController spiralObjectController = GetInactiveAbilityObject() as IceSpiralObjectController;
            if (spiralObjectController == null) yield break;
            spiralObjectController.Execute(transform.position, Quaternion.identity);
        }
    }
}