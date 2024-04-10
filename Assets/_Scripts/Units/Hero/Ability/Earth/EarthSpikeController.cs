using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class EarthSpikeController : Controller
    {
        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            EarthSpikeObjectController spikeObjectController = GetInactiveAbilityObject() as EarthSpikeObjectController;
            if (spikeObjectController == null) yield break;
            Transform t = HeroController.Animator.transform;
            spikeObjectController.Execute(t.position + t.forward * 3, t.rotation);
        }
    }
}