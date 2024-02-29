using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleProjectileHoming : BattleProjectile
    {
        BattleManager _battleManager;

        Rigidbody _rb;

        [SerializeField] float _rotateSpeed = 0.5f;

        BattleEntity _target;

        IEnumerator _homingCoroutine;

        float _endTime;

        public override void Initialize(int team)
        {
            base.Initialize(team);

            _battleManager = BattleManager.Instance;
            _rb = GetComponent<Rigidbody>();
        }

        public void StartHoming(Ability ability)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            Ability = ability;
            EnableProjectile();

            _endTime = UnityEngine.Time.time + Ability.GetDuration();

            _homingCoroutine = HomingCoroutine();
            StartCoroutine(_homingCoroutine);
        }

        IEnumerator HomingCoroutine()
        {
            if (_endTime < UnityEngine.Time.time) yield break;
            StartCoroutine(BreakHomingCoroutine());
            yield return GoForward(0.5f);

            while (_battleManager.GetOpponents(Team).Count == 0)
                yield return GoForward(0.5f);

            _target = GetClosestEntity(_battleManager.GetOpponents(Team));
            while (_target != null)
            {
                if (_target.IsDead) break;
                Transform t = transform;
                Vector3 forward = t.forward;
                _rb.velocity = forward * Speed;
                Vector3 direction = _target.transform.position - t.position;
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
            yield return new WaitForSeconds(Ability.GetDuration());
            if (_homingCoroutine != null) HitConnected();
        }

        IEnumerator GoForward(float timeInSeconds)
        {
            _rb.velocity = transform.forward * Speed;
            yield return new WaitForSeconds(timeInSeconds);
        }

        BattleEntity GetClosestEntity(List<BattleEntity> battleEntities)
        {
            float minDistance = Mathf.Infinity;
            BattleEntity closestEntity = null;
            foreach (BattleEntity entity in battleEntities)
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