using System;
using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Projectile
{
    public class ProjectileController : MonoBehaviour
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
        protected Attack.Attack Attack;

        protected float Time;
        protected Vector3 Direction;

        protected bool IsHitConnected;

        public event Action OnExplode;

        public virtual void Initialize(int team, Attack.Attack attack)
        {
            Team = team;
            Attack = attack;

            _audioManager = AudioManager.Instance;
            _collider = GetComponent<SphereCollider>();

            if (team == 0)
                _collider.excludeLayers = LayerMask.GetMask("Player", "Team 0", "Ability");
            if (team == 1)
                _collider.excludeLayers = LayerMask.GetMask("Team 1");
        }

        public void Shoot(Vector3 dir)
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
            if (_shootSound != null)
                _audioManager.PlaySfx(_shootSound, transform.position);
        }

        protected IEnumerator ShootInDirectionCoroutine(Vector3 dir)
        {
            Direction = dir;
            Direction.Normalize();
            Direction.y = 0;

            float t = 0;
            while (t <= Time)
            {
                transform.position += Direction * (Speed * UnityEngine.Time.fixedDeltaTime);
                t += UnityEngine.Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return Explode(transform.position);
        }

        protected virtual void HitTarget(UnitController target)
        {
            StartCoroutine(target.GetHit(Attack));
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (IsHitConnected) return;

            if (collision.gameObject.layer == Tags.UnpassableLayer ||
                collision.gameObject.layer == Tags.BattleInteractableLayer)
            {
                CollideWithUnpassable(collision);
                return;
            }

            if (collision.gameObject.TryGetComponent(out UnitController unitController))
            {
                if (!IsTargetValid(unitController)) return;

                HitConnected();
                HitTarget(unitController);
                return;
            }

            if (collision.gameObject.TryGetComponent(out BreakableVaseController bbv))
            {
                HitConnected();
                bbv.TriggerBreak();
            }
        }

        protected virtual void CollideWithUnpassable(Collision collision)
        {
            HitConnected();
        }

        protected bool IsTargetValid(UnitController unitController)
        {
            if (unitController.IsDead) return false;
            if (Team == unitController.Team) return false;
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

        protected virtual IEnumerator Explode(Vector3 position)
        {
            Gfx.SetActive(false);
            if (ExplosionSound != null)
                _audioManager.PlaySfx(ExplosionSound, position);
            Explosion.SetActive(true);

            yield return new WaitForSeconds(3f);

            transform.DOKill(transform);
            gameObject.SetActive(false);
            OnExplode?.Invoke();
        }
    }
}