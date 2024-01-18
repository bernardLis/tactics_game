using System.Collections;


using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleEarthSlash : BattleAbilityObject
    {
        [SerializeField] GameObject _effect;
        [SerializeField] GameObject _col;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localScale = Vector3.one * _ability.GetScale();
        }

        protected override void OnAbilityLevelUp()
        {
            transform.localScale = Vector3.one * _ability.GetScale();
        }

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            base.Execute(pos, rot);
        
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            _effect.SetActive(true);
            _col.SetActive(true);

            Vector3 colliderRotation = new(90f, 0f, -45f);
            _col.transform.DOLocalRotate(colliderRotation, _ability.GetDuration())
                .OnComplete(() =>
                {
                    _col.SetActive(false);
                });

            yield return new WaitForSeconds(1f);
            _effect.SetActive(false);
            gameObject.SetActive(false);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                StartCoroutine(battleEntity.GetHit(_ability));
            }
        }
    }
}
