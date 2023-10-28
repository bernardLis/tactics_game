using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleProjectile : MonoBehaviour
{
    AudioManager _audioManager;

    [SerializeField] Sound _explosionSound;
    [SerializeField] Sound _shootSound;

    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _explosion;

    [SerializeField] protected int _speed;

    protected int _team;
    protected EntityFight _shooter;
    protected BattleEntity _target;

    bool _hitConnected;
    public virtual void Initialize(int Team)
    {
        _team = Team;
        _audioManager = AudioManager.Instance;
        _audioManager.PlaySFX(_shootSound, transform.position);
    }

    public void Shoot(EntityFight shooter, BattleEntity target, float power)
    {
        _shooter = shooter;
        _target = target;
        StartCoroutine(ShootCoroutine(shooter.AttackRange.GetValue(), target, power));
    }

    IEnumerator ShootCoroutine(float range, BattleEntity target, float power)
    {
        if (target == null) DestroySelf(transform.position);

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

        yield return DestroySelf(transform.position);
    }

    protected virtual IEnumerator HitTarget(BattleEntity target)
    {
        GetComponent<SphereCollider>().enabled = false;
        if (_shooter != null) StartCoroutine(target.GetHit(_shooter));

        yield return DestroySelf(transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hitConnected) return;

        if (collision.gameObject.layer == Tags.BattleObstacleLayer ||
            collision.gameObject.layer == Tags.BattleFloorLayer)
        {
            _hitConnected = true;
            StopAllCoroutines();
            StartCoroutine(DestroySelf(transform.position));
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
    }

    bool IsTargetValid(BattleEntity battleEntity)
    {
        if (battleEntity.IsDead) return false;
        // if (battleEntity is BattleHero) return false; // HERE: projectile for now...
        if (_team == battleEntity.Team) return false;

        return true;
    }

    public IEnumerator DestroySelf(Vector3 position)
    {
        _gfx.SetActive(false);
        _audioManager.PlaySFX(_explosionSound, position);
        _explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        transform.DOKill(transform);
        Destroy(gameObject);
    }
}
