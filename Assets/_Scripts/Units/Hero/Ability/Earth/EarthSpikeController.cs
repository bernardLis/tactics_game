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
            transform.localPosition = new Vector3(0f, 0f, 4f); // it is where the effect spawns...
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            EarthSpikeObjectController spikeObjectController = GetInactiveAbilityObject() as EarthSpikeObjectController;
            if (spikeObjectController == null) yield break;
            spikeObjectController.Execute(transform.position, _heroController.transform.rotation);
        }
    }
}