using System.Collections;

using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWindTornado : BattleAbilityObject
    {
        float _speed = 5f;

        public override void Execute(Vector3 pos, Quaternion q)
        {
            pos.y = 0;
            base.Execute(pos, q);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            transform.localScale = Vector3.one * _ability.GetScale();

            float elapsedTime = 0;
            while (elapsedTime < _ability.GetDuration())
            {
                transform.position += _speed * Time.fixedDeltaTime * transform.forward;
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
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
