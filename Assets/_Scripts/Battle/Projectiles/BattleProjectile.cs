using System;
using System.Collections;






using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleProjectile : MonoBehaviour
    {
        protected AudioManager _audioManager;

        [SerializeField] protected Sound _explosionSound;
        [SerializeField] Sound _shootSound;

        [SerializeField] protected GameObject _gfx;
        [SerializeField] protected GameObject _explosion;

        [SerializeField] protected float _speed;
        protected SphereCollider _collider;

        protected int _team;
        protected EntityFight _shooter;
        protected Ability _ability;

        protected float _time;

        protected bool _hitConnected;

        public event Action OnExplode;
        public virtual void Initialize(int Team)
        {
            _team = Team;
            _audioManager = AudioManager.Instance;
            _collider = GetComponent<SphereCollider>();
        }

        public void Shoot(Ability ability, Vector3 dir)
        {
            _ability = ability;
            _time = _ability.GetDuration();
            BaseShoot(dir);
        }

        public void Shoot(EntityFight shooter, Vector3 dir)
        {
            _shooter = shooter;
            // TODO: idk if this will work for range 
            _time = _shooter.AttackRange.GetValue() / _speed;
            BaseShoot(dir);
        }

        void BaseShoot(Vector3 dir)
        {
            float targetScale = transform.localScale.x;
            transform.localScale = transform.localScale * 0.5f;
            transform.DOScale(targetScale, 1f);
            transform.LookAt(transform.position + dir);

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(dir));
        }

        protected void EnableProjectile()
        {
            gameObject.SetActive(true);
            _collider.enabled = true;
            _hitConnected = false;
            _gfx.SetActive(true);
            _explosion.SetActive(false);
            _audioManager.PlaySFX(_shootSound, transform.position);
        }

        protected IEnumerator ShootInDirectionCoroutine(Vector3 dir)
        {
            dir.Normalize();
            float t = 0;
            while (t <= _time)
            {
                transform.position += dir * _speed * Time.fixedDeltaTime;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            yield return Explode(transform.position);
        }

        protected virtual void HitTarget(BattleEntity target)
        {
            if (_shooter != null) StartCoroutine(target.GetHit(_shooter));
            if (_ability != null) StartCoroutine(target.GetHit(_ability));
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (_hitConnected) return;

            if (collision.gameObject.layer == Tags.BattleObstacleLayer ||
                collision.gameObject.layer == Tags.BattleInteractableLayer)
            {
                HitConnected();
                return;
            }

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (!IsTargetValid(battleEntity)) return;

                HitConnected();
                HitTarget(battleEntity);
                return;
            }

            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
            {
                HitConnected();
                bbv.TriggerBreak();
                return;
            }
        }

        protected bool IsTargetValid(BattleEntity battleEntity)
        {
            if (battleEntity.IsDead) return false;
            if (_team == battleEntity.Team) return false;
            return true;
        }

        protected void HitConnected()
        {
            if (!gameObject.activeSelf) return;
            _collider.enabled = false;
            _hitConnected = true;
            StopAllCoroutines();
            StartCoroutine(Explode(transform.position));
        }

        public virtual IEnumerator Explode(Vector3 position)
        {
            _gfx.SetActive(false);
            _audioManager.PlaySFX(_explosionSound, position);
            _explosion.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            transform.DOKill(transform);
            gameObject.SetActive(false);
            OnExplode?.Invoke();
        }
    }
}
