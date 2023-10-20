using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProjectileHoming : BattleProjectile
{
    BattleManager _battleManager;

    Rigidbody _rb;

    [SerializeField] int _homingDurationSeconds = 5;
    [SerializeField] float _rotateSpeed = 0.03f;

    Ability _ability;

    IEnumerator _homingCoroutine;

    // TBH, it does not have that much in common with projectile, 
    // and this set-up makes it unusable by creatures
    // it is a first try tho. I can make it better later xD
    public override void Initialize(int team)
    {
        base.Initialize(team);

        _battleManager = BattleManager.Instance;
        _rb = GetComponent<Rigidbody>();
    }

    public void StartHoming(Ability ability)
    {
        _ability = ability;

        _homingCoroutine = HomingCoroutine();
        StartCoroutine(_homingCoroutine);

        Invoke("StopHoming", _homingDurationSeconds);
    }

    IEnumerator HomingCoroutine()
    {
        List<BattleEntity> battleEntities = _battleManager.GetOpponents(_team);
        if (battleEntities.Count == 0)
        {
            yield return GoForward(0.5f);
            yield return HomingCoroutine();
            yield break;
        }
        // get closest enemy
        _target = GetClosestEntity(battleEntities);

        yield return GoForward(0.5f);

        // then start homing 
        while (_target != null)
        {
            if (_target.IsDead) break;

            Vector3 direction = _target.transform.position - transform.position;
            direction.Normalize();

            Vector3 amountToRotate = Vector3.Cross(direction, transform.forward)
                                    * Vector3.Angle(transform.forward, direction);

            _rb.angularVelocity = -amountToRotate * _rotateSpeed;

            _rb.velocity = transform.forward * _speed;

            yield return new WaitForFixedUpdate();
        }

        // if the target dies or disappears
        yield return HomingCoroutine();
    }

    IEnumerator GoForward(float timeInSeconds)
    {
        _rb.velocity = transform.forward * _speed;
        yield return new WaitForSeconds(timeInSeconds);
    }

    protected override IEnumerator HitTarget(BattleEntity target)
    {
        if (_ability != null) StartCoroutine(target.GetHit(_ability));

        yield return DestroySelf(transform.position);
    }



    BattleEntity GetClosestEntity(List<BattleEntity> battleEntities)
    {
        float minDistance = Mathf.Infinity;
        BattleEntity closestEntity = null;
        foreach (BattleEntity entity in battleEntities)
        {
            float distance = Vector3.Distance(transform.position, entity.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEntity = entity;
            }
        }
        return closestEntity;
    }


    void StopHoming()
    {
        StopCoroutine(_homingCoroutine);
        StartCoroutine(DestroySelf(transform.position));
    }
}
