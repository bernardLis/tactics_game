using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class BattleProjectile : MonoBehaviour
    {
        AudioManager _audioManager;

        [FormerlySerializedAs("_explosionSound")] [SerializeField]
        protected Sound ExplosionSound;

        [SerializeField] Sound _shootSound;

        [SerializeField] protected GameObject Gfx;
        [SerializeField] protected GameObject Explosion;

        [FormerlySerializedAs("_speed")] [SerializeField]
        protected float Speed;

        SphereCollider _collider;

        protected int Team;
        protected EntityFight Shooter;
        protected Ability Ability;

        protected float Time;

        protected bool IsHitConnected;

        public event Action OnExplode;

        public virtual void Initialize(int team)
        {
            Team = team;
            _audioManager = AudioManager.Instance;
            _collider = GetComponent<SphereCollider>();
        }

        public void Shoot(Ability ability, Vector3 dir)
        {
            Ability = ability;
            Time = Ability.GetDuration();
            BaseShoot(dir);
        }

        public void Shoot(EntityFight shooter, Vector3 dir)
        {
            Shooter = shooter;
            // TODO: idk if this will work for range 
            Time = Shooter.AttackRange.GetValue() / Speed;
            BaseShoot(dir);
        }

        void BaseShoot(Vector3 dir)
        {
            Transform t = transform;
            Vector3 scale = t.localScale;
            float targetScale = scale.x;

            scale *= 0.5f;
            t.localScale = scale;
            transform.DOScale(targetScale, 1f);
            transform.LookAt(t.position + dir);

            EnableProjectile();
            StartCoroutine(ShootInDirectionCoroutine(dir));
        }

        protected void EnableProjectile()
        {
            gameObject.SetActive(true);
            _collider.enabled = true;
            IsHitConnected = false;
            Gfx.SetActive(true);
            Explosion.SetActive(false);
            _audioManager.PlaySFX(_shootSound, transform.position);
        }

        protected IEnumerator ShootInDirectionCoroutine(Vector3 dir)
        {
            dir.Normalize();
            float t = 0;
            while (t <= Time)
            {
                transform.position += dir * (Speed * UnityEngine.Time.fixedDeltaTime);
                t += UnityEngine.Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return Explode(transform.position);
        }

        protected virtual void HitTarget(BattleEntity target)
        {
            if (Shooter != null) StartCoroutine(target.GetHit(Shooter));
            if (Ability != null) StartCoroutine(target.GetHit(Ability));
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (IsHitConnected) return;

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

            if (!collision.gameObject.TryGetComponent(out BattleBreakableVase bbv)) return;
            HitConnected();
            bbv.TriggerBreak();
        }

        protected bool IsTargetValid(BattleEntity battleEntity)
        {
            if (battleEntity.IsDead) return false;
            if (Team == battleEntity.Team) return false;
            return true;
        }

        protected void HitConnected()
        {
            if (!gameObject.activeSelf) return;
            _collider.enabled = false;
            IsHitConnected = true;
            StopAllCoroutines();
            StartCoroutine(Explode(transform.position));
        }

        public virtual IEnumerator Explode(Vector3 position)
        {
            Gfx.SetActive(false);
            _audioManager.PlaySFX(ExplosionSound, position);
            Explosion.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            transform.DOKill(transform);
            gameObject.SetActive(false);
            OnExplode?.Invoke();
        }
    }
}