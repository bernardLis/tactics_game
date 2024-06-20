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

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (col.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                StartCoroutine(battleEntity.GetHit(Ability.GetCurrentLevel()));
            }
        }

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
            if (Ability.ExecuteSound != null)
            {
                AudioManager.CreateSound()
                    .WithSound(Ability.ExecuteSound)
                    .WithPosition(transform.position)
                    .Play();
            }

            Vector3 colliderRotation = new(90f, 0f, -45f);
            _col.transform.DOLocalRotate(colliderRotation, Ability.GetDuration() - 0.2f)
                .OnComplete(() => { _col.SetActive(false); });

            yield return new WaitForSeconds(1f);
            _effect.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}