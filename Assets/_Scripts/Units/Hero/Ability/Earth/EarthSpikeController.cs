using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class EarthSpikeController : Controller
    {
        HeroController _heroController;

        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            _heroController = BattleManager.HeroController;
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            EarthSpikeObjectController spikeObjectController = GetInactiveAbilityObject() as EarthSpikeObjectController;
            if (spikeObjectController == null) yield break;
            Transform t = _heroController.Animator.transform;
            spikeObjectController.Execute(t.position + t.forward * 3, t.rotation);
        }
    }
}