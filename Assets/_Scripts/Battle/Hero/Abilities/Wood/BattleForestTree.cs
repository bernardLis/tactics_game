using System.Collections;


using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleForestTree : BattleAbilityObjectDmgOverTime
    {
        [SerializeField] GameObject[] _treeGFX;
        [SerializeField] GameObject _effect;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            Disable();
            transform.localScale = Vector3.zero;
        }

        public override void Execute(Vector3 pos, Quaternion rot)
        {
            pos.y = 0;
            _treeGFX[Random.Range(0, _treeGFX.Length)].SetActive(true);

            _effect.SetActive(true);
            transform.DOScale(1, 0.5f)
                .SetDelay(0.2f);
            base.Execute(pos, rot);
            DamageOnInception();
        }

        void DamageOnInception()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (Collider c in colliders)
            {
                if (c.TryGetComponent(out BattleBreakableVase bbv))
                {
                    bbv.TriggerBreak();
                    continue;
                }
                if (c.TryGetComponent(out BattleEntity entity))
                {
                    if (entity.Team == 0) continue; // TODO: hardcoded team number
                    StartCoroutine(entity.GetHit(_ability));
                }
            }
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            yield return DamageCoroutine(Time.time + _ability.GetDuration());
            transform.DOScale(0, 0.5f)
                .OnComplete(Disable);
        }

        protected override IEnumerator DamageCoroutine(float endTime, float interval = 0.5F)
        {
            while (Time.time < endTime)
            {
                if (EntitiesInCollider.Count > 0)
                {
                    BattleEntity entity = EntitiesInCollider[Random.Range(0, EntitiesInCollider.Count)];
                    // rotate to face entity
                    transform.DOLookAt(entity.transform.position, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    // punch rotation to 90 degrees forward
                    Vector3 originalRot = transform.eulerAngles;
                    Vector3 rot = transform.eulerAngles;
                    rot.z = 75;
                    transform.DORotate(rot, 0.1f)
                        .OnComplete(() => transform.DORotate(originalRot, 0.1f));
                    StartCoroutine(entity.GetHit(_ability));
                }
                yield return new WaitForSeconds(0.7f);
            }
        }

        void Disable()
        {
            foreach (GameObject g in _treeGFX)
                g.SetActive(false);
            _effect.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
