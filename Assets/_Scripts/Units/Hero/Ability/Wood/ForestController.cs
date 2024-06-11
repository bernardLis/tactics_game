using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ForestController : Controller
    {
        [SerializeField] private GameObject _effect;

        private readonly float _radius = 12f;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Handles.DrawWireDisc(transform.position, Vector3.up, _radius, 1f);
        }
#endif

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0, 0f, 0f);
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
                Vector3 pos = ArenaManager.GetRandomPositionWithinRange(transform.position,
                    _radius * Ability.GetScale());
                ForestTreeObjectController treeObjectController =
                    GetInactiveAbilityObject() as ForestTreeObjectController;
                if (treeObjectController != null) treeObjectController.Execute(pos, Quaternion.identity);
            }

            yield return new WaitForSeconds(3f);
            _effect.SetActive(false);
        }
    }
}