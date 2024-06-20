using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WindTornadoController : Controller
    {
        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0.5f, 1f, 0.5f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();

            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                Vector3 currentPosition = transform.position;
                Vector3 pos = new(currentPosition.x, 0, currentPosition.z);
                Quaternion look = Quaternion.LookRotation(GetPositionTowardsCursor(),
                    Vector3.up);

                WindTornadoObjectController tornadoObjectController =
                    GetInactiveAbilityObject() as WindTornadoObjectController;
                if (tornadoObjectController != null) tornadoObjectController.Execute(pos, look);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}