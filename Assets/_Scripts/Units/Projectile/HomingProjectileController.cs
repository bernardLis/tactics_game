using System.Collections;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class HomingProjectileController : ProjectileController
    {
        [SerializeField] float _rotateSpeed = 0.5f;

        Ability _ability;
        BattleManager _battleManager;

        float _endTime;
        FightManager _fightManager;

        IEnumerator _homingCoroutine;

        Rigidbody _rb;

        UnitController _target;

        public override void Initialize(int team, Attack.Attack attack)
        {
            base.Initialize(team, attack);

            _battleManager = BattleManager.Instance;
            _fightManager = _battleManager.GetComponent<FightManager>();
            _rb = GetComponent<Rigidbody>();
        }

        public void StartHoming(Ability ability)
        {
            _ability = ability;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            EnableProjectile();

            _endTime = Time.time + _ability.GetDuration();

            _homingCoroutine = HomingCoroutine();
            StartCoroutine(_homingCoroutine);
        }

        IEnumerator HomingCoroutine()
        {
            if (_endTime < Time.time) yield break;
            StartCoroutine(BreakHomingCoroutine());
            yield return GoForward(0.5f);

            while (_fightManager.GetOpponents(Team).Count == 0)
                yield return GoForward(0.5f);

            _target = GetClosestEntity(_fightManager.GetOpponents(Team));
            while (_target != null)
            {
                if (_target.IsDead) break;
                Transform t = transform;
                Vector3 forward = t.forward;
                _rb.velocity = forward * Speed;
                Vector3 direction = _target.Collider.bounds.center - t.position;
                direction.Normalize();

                Vector3 amountToRotate = Vector3.Cross(direction, forward)
                                         * Vector3.Angle(forward, direction);
                _rb.angularVelocity = -amountToRotate * _rotateSpeed;
                yield return new WaitForFixedUpdate();
            }

            // if the target dies or disappears
            yield return HomingCoroutine();
        }

        IEnumerator BreakHomingCoroutine()
        {
            yield return new WaitForSeconds(_ability.GetDuration());
            if (_homingCoroutine != null) HitConnected();
        }

        IEnumerator GoForward(float timeInSeconds)
        {
            _rb.velocity = transform.forward * Speed;
            yield return new WaitForSeconds(timeInSeconds);
        }

        UnitController GetClosestEntity(List<UnitController> battleEntities)
        {
            float minDistance = Mathf.Infinity;
            UnitController closestEntity = null;
            foreach (UnitController entity in battleEntities)
            {
                Vector3 delta = entity.transform.position - transform.position;
                float sqrDistance = delta.sqrMagnitude;
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    closestEntity = entity;
                }
            }

            return closestEntity;
        }
    }
}