using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ForestController : Controller
    {
        [SerializeField] GameObject _effect;

        readonly float _radius = 12f;

        public override void Initialize(Ability ability, bool startAbility = true)
        {
            base.Initialize(ability, startAbility);
            transform.localPosition = new Vector3(0, 0f, 0f);
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            yield return base.ExecuteAbilityCoroutine();
            _effect.SetActive(true);
            if (Ability.ExecuteSound != null)
                AudioManager.PlaySfx(Ability.ExecuteSound, transform.position);

            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < Ability.GetAmount(); i++)
            {
                Vector3 pos = AreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position,
                    _radius * Ability.GetScale());
                ForestTreeObjectController treeObjectController =
                    GetInactiveAbilityObject() as ForestTreeObjectController;
                if (treeObjectController != null) treeObjectController.Execute(pos, Quaternion.identity);
            }

            yield return new WaitForSeconds(3f);
            _effect.SetActive(false);
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            Handles.DrawWireDisc(transform.position, Vector3.up, _radius, 1f);
        }
#endif
    }
}