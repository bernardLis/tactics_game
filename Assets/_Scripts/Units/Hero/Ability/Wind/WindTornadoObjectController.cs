using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class WindTornadoObjectController : ObjectController
    {
        readonly float _originalSpeed = 7f;
        float _currentSpeed = 7f;

        bool _isUnpassableCollisionActive;

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == Tags.UnpassableLayer && _isUnpassableCollisionActive)
                _currentSpeed = 0;

            if (col.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (col.gameObject.TryGetComponent(out UnitController unitController))
            {
                if (!unitController.isActiveAndEnabled) return;
                if (unitController.Team == 0) return; // TODO: hardcoded team number
                if (unitController.IsDead) return;
                StartCoroutine(unitController.GetHit(Ability.GetCurrentLevel()));
            }
        }

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

            if (Ability.ExecuteSound != null)
            {
                SoundEmitter = AudioManager.CreateSound()
                    .WithSound(Ability.ExecuteSound)
                    .WithParent(transform)
                    .Play();
            }

            float elapsedTime = 0;
            while (elapsedTime < Ability.GetDuration())
            {
                if (elapsedTime > 1f && !_isUnpassableCollisionActive)
                    _isUnpassableCollisionActive = true;

                t.position += _currentSpeed * Time.fixedDeltaTime * t.forward;
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            if (SoundEmitter != null)
            {
                SoundEmitter.Stop();
                SoundEmitter.transform.parent = AudioManager.transform;
                SoundEmitter = null;
            }

            transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        public override void DisableSelf()
        {
            if (SoundEmitter != null)
            {
                SoundEmitter.Stop();
                SoundEmitter.transform.parent = AudioManager.transform;
                SoundEmitter = null;
            }

            base.DisableSelf();
        }
    }
}