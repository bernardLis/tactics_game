using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ForestTreeObjectController : ObjectControllerDmgOverTime
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
                if (c.TryGetComponent(out BreakableVaseController bbv))
                {
                    bbv.TriggerBreak();
                    continue;
                }
                if (c.TryGetComponent(out UnitController entity))
                {
                    if (entity.Team == 0) continue; // TODO: hardcoded team number
                    StartCoroutine(entity.GetHit(Ability));
                }
            }
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            yield return DamageCoroutine(Time.time + Ability.GetDuration());
            transform.DOScale(0, 0.5f)
                .OnComplete(Disable);
        }

        protected override IEnumerator DamageCoroutine(float endTime, float interval = 0.5F)
        {
            while (Time.time < endTime)
            {
                if (UnitsInCollider.Count > 0)
                {
                    UnitController entity = UnitsInCollider[Random.Range(0, UnitsInCollider.Count)];
                    // rotate to face unit
                    transform.DOLookAt(entity.transform.position, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    // punch rotation to 90 degrees forward
                    Vector3 originalRot = transform.eulerAngles;
                    Vector3 rot = transform.eulerAngles;
                    rot.z = 75;
                    transform.DORotate(rot, 0.1f)
                        .OnComplete(() => transform.DORotate(originalRot, 0.1f));
                    StartCoroutine(entity.GetHit(Ability));
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
