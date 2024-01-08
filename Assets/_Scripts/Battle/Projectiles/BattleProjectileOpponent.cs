using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BattleProjectileOpponent : BattleProjectile
{
    BattleEntity _battleEntity;
    protected float _time;
    int _power;

    protected Vector3 _direction;

    public event Action OnDestroy;
    public override void Initialize(int Team)
    {
        base.Initialize(Team);

        ResetProjectile();
    }

    void ResetProjectile()
    {
        _gfx.SetActive(true);
        _explosion.SetActive(false);
        _hitConnected = false;
        _collider.enabled = true;

    }

    public virtual void Shoot(BattleEntity shooter, Vector3 dir, float time, int power)
    {
        _battleEntity = shooter;
        _time = time;
        _power = power;
        _direction = dir;

        gameObject.SetActive(true);
        StartCoroutine(ShootCoroutine());
    }

    IEnumerator ShootCoroutine()
    {
        float t = 0;
        while (t <= _time)
        {
            transform.position += _direction * _speed * Time.fixedDeltaTime;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return Explode(transform.position);
    }

    protected override IEnumerator HitTarget(BattleEntity target)
    {
        _collider.enabled = false;
        target.BaseGetHit(_power, Color.black);

        yield return Explode(transform.position);
    }


    public override IEnumerator Explode(Vector3 position)
    {
        _gfx.SetActive(false);
        _audioManager.PlaySFX(_explosionSound, position);
        _explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        OnDestroy?.Invoke();
        transform.DOKill(transform);
        gameObject.SetActive(false);
    }


}
