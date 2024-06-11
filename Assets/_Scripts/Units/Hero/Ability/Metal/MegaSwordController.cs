using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class MegaSwordController : Controller
    {
        readonly float _elevationOffset = 0.5f;

        readonly float _rotationSpeed = 2;
        float _angle;
        float _circleRadius = 3;
        bool _isActive;

        Vector3 _positionOffset;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                StartCoroutine(battleEntity.GetHit(Ability.GetCurrentLevel()));
            }
        }


        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            transform.localPosition = new(0, 0.5f, 0f);

            Ability.OnLevelUp += ScaleAbility;
            ScaleAbility();
        }

        void ScaleAbility()
        {
            transform.DOScale(Vector3.one * Ability.GetScale(), 0.5f).SetEase(Ease.InOutSine);
            _circleRadius = 3 + Ability.GetScale();
        }

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            // this ability is continuous, fire it only once.
            if (_isActive) yield break;
            _isActive = true;

            while (_isActive)
            {
                _positionOffset.Set(
                    Mathf.Cos(_angle) * _circleRadius,
                    _elevationOffset,
                    Mathf.Sin(_angle) * _circleRadius
                );
                transform.position = HeroController.transform.position + _positionOffset;

                Vector3 lookRotation = Quaternion.LookRotation(_positionOffset).eulerAngles;
                lookRotation.x = 0;
                transform.rotation = Quaternion.Euler(lookRotation);

                _angle += Time.fixedDeltaTime * _rotationSpeed;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}