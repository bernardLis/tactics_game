using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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
    protected BattleEntity _target;

    protected bool _hitConnected;

    public event Action OnExplode;
    public virtual void Initialize(int Team)
    {
        _team = Team;
        _audioManager = AudioManager.Instance;
        _collider = GetComponent<SphereCollider>();
    }

    void EnableProjectile()
    {
        gameObject.SetActive(true);
        _hitConnected = false;
        _gfx.SetActive(true);
        _explosion.SetActive(false);
        _audioManager.PlaySFX(_shootSound, transform.position);
    }

    public void ShootInDirection(Ability ability, Vector3 dir)
    {
        EnableProjectile();

        _ability = ability;
        StartCoroutine(ShootInDirectionCoroutine(dir));
        _time = _ability.GetDuration();
    }

    IEnumerator ShootInDirectionCoroutine(Vector3 dir)
    {
        Debug.Log($"dir {dir} & speed: {_speed}");
        float targetScale = transform.localScale.x;
        transform.localScale = transform.localScale * 0.5f;
        transform.DOScale(targetScale, 1f);
        transform.LookAt(transform.position + dir);

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

    public void ShootAt(EntityFight shooter, BattleEntity target, float power)
    {
        EnableProjectile();
        _shooter = shooter;
        _target = target;
        StartCoroutine(ShootAtCoroutine(shooter.AttackRange.GetValue(), target, power));
    }

    IEnumerator ShootAtCoroutine(float range, BattleEntity target, float power)
    {
        if (target == null)
        {
            Explode(transform.position);
            yield break;
        }

        float targetScale = transform.localScale.x;
        transform.localScale = transform.localScale * 0.5f;
        transform.DOScale(targetScale, 1f);
        transform.LookAt(target.transform);

        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.Collider.bounds.center;
        Vector3 dir = (finalPos - startingPos).normalized;
        Vector3 destination = startingPos + dir * range;

        float t = 0;
        float step = _speed / range * Time.fixedDeltaTime;

        while (t <= 1.0f)
        {
            t += step;
            Vector3 newPos = Vector3.Lerp(startingPos, destination, t);
            transform.position = newPos;
            yield return new WaitForFixedUpdate();
        }

        yield return Explode(transform.position);
    }

    protected virtual IEnumerator HitTarget(BattleEntity target)
    {
        _collider.enabled = false;
        if (_shooter != null) StartCoroutine(target.GetHit(_shooter));
        if (_ability != null) StartCoroutine(target.GetHit(_ability));

        yield return Explode(transform.position);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (_hitConnected) return;

        Debug.Log($"Projectile {gameObject.name} collided with {collision.gameObject.name}");
        Debug.Log($"layer {collision.gameObject.layer}");

        if (collision.gameObject.layer == Tags.BattleObstacleLayer ||
            collision.gameObject.layer == Tags.BattleInteractableLayer)
        {
            _hitConnected = true;
            StopAllCoroutines();
            StartCoroutine(Explode(transform.position));
            return;
        }

        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (!IsTargetValid(battleEntity)) return;

            _hitConnected = true;
            StopAllCoroutines();
            StartCoroutine(HitTarget(battleEntity));
            return;
        }

        if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
        {
            _hitConnected = true;
            StopAllCoroutines();
            bbv.TriggerBreak();
            StartCoroutine(Explode(transform.position));
            return;
        }
    }

    protected bool IsTargetValid(BattleEntity battleEntity)
    {
        if (battleEntity.IsDead) return false;
        // if (battleEntity is BattleHero) return false; // HERE: projectile for now...
        if (_team == battleEntity.Team) return false;
        Debug.Log($"valid target");
        return true;
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
