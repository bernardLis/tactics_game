using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class EarthSlashObjectController : ObjectController
    {
        [SerializeField] GameObject _effect;
        [SerializeField] GameObject _col;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localScale = Vector3.one * Ability.GetScale();
        }

        protected override void OnAbilityLevelUp()
        {
            transform.localScale = Vector3.one * Ability.GetScale();
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _effect.SetActive(true);
            _col.SetActive(true);

            Vector3 colliderRotation = new(90f, 0f, -45f);
            _col.transform.DOLocalRotate(colliderRotation, Ability.GetDuration())
                .OnComplete(() =>
                {
                    _col.SetActive(false);
                });

            yield return new WaitForSeconds(1f);
            _effect.SetActive(false);
            gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (col.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                StartCoroutine(battleEntity.GetHit(Ability));
            }
        }
    }
}
