using System.Collections;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class MeteorsController : Controller
    {
        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new Vector3(-0.5f, 1f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                // random position within circle radius
                Vector3 pos = ArenaManager.GetRandomPositionInArena();
                MeteorsObjectController meteorObjectController = GetInactiveAbilityObject() as MeteorsObjectController;
                if (meteorObjectController != null) meteorObjectController.Execute(pos, Quaternion.identity);
            }
        }
    }
}