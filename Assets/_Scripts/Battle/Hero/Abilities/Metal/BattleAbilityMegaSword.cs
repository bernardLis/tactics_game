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

    Vector3 positionOffset;
    float angle;

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

    protected override IEnumerator FireAbilityCoroutine()
    {
        // this ability is continuous, fire it only once.
        if (_isActive) yield break;
        _isActive = true;

        while (_isActive)
        {
            positionOffset.Set(
                            Mathf.Cos(angle) * _circleRadius,
                            _elevationOffset,
                            Mathf.Sin(angle) * _circleRadius
                        );
            transform.position = _hero.transform.position + positionOffset;
            // rotate to face forward

            transform.rotation = Quaternion.LookRotation(positionOffset);

            angle += Time.fixedDeltaTime * _rotationSpeed;
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
