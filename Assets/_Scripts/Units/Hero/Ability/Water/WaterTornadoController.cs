using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WaterTornadoController : Controller
    {
        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0.5f, 1f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                Vector3 pos = ArenaManager.GetRandomPositionInArena();
                if (OpponentTracker.OpponentsInRange.Count > 0)
                    pos = OpponentTracker.GetRandomOpponentPosition();
                WaterTornadoObjectController tornadoObjectController =
                    GetInactiveAbilityObject() as WaterTornadoObjectController;
                if (tornadoObjectController != null) tornadoObjectController.Execute(pos, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}