using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleAbilityMegaSword : BattleAbility
{

    bool _isActive;

    float _rotationSpeed = 2;
    float _circleRadius = 3;
    float _elevationOffset = 0.5f;

    Vector3 _positionOffset;
    float _angle;

    BattleHero _hero;


    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0, 0.5f, 0f);

        _ability.OnLevelUp += ScaleAbility;
        _hero = BattleManager.Instance.BattleHero;
        ScaleAbility();

        // with scale should the radius be larger?
    }

    void ScaleAbility()
    {
        transform.DOScale(Vector3.one * _ability.GetScale(), 0.5f).SetEase(Ease.InOutSine);
        _circleRadius = 3 + _ability.GetScale();
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
            transform.position = _hero.transform.position + _positionOffset;
            // rotate to face forward

            // Vector3 rotation = positionOffset;
            // rotation.x = 0;

            Vector3 lookRotation = Quaternion.LookRotation(_positionOffset).eulerAngles;
            lookRotation.x = 0;


            transform.rotation = Quaternion.Euler(lookRotation);

            _angle += Time.fixedDeltaTime * _rotationSpeed;
            yield return new WaitForFixedUpdate();
        }
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
