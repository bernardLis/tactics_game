using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleWindTornado : BattleAbilityObject
    {
        readonly float _originalSpeed = 7f;
        float _currentSpeed = 7f;

        bool _isUnpassableCollisionActive;

        public override void Execute(Vector3 pos, Quaternion q)
        {
            pos.y = 0;
            base.Execute(pos, q);
        }

        protected override IEnumerator ExecuteCoroutine()
        {
            Transform t = transform;
            t.localScale = Vector3.one * Ability.GetScale();
            
            _currentSpeed = _originalSpeed;
            _isUnpassableCollisionActive = false;

            float elapsedTime = 0;
            while (elapsedTime < Ability.GetDuration())
            {
                if (elapsedTime > 1f && !_isUnpassableCollisionActive)
                    _isUnpassableCollisionActive = true;

                t.position += _currentSpeed * Time.fixedDeltaTime * t.forward;
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Tags.UnpassableLayer && _isUnpassableCollisionActive)
                _currentSpeed = 0;

            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                StartCoroutine(battleEntity.GetHit(Ability));
            }
        }
    }
}