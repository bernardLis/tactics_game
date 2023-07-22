using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    AudioManager _audioManager;

    [SerializeField] Sound _explosionSound;
    [SerializeField] Sound _shootSound;

    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _explosion;

    [SerializeField] int _speed;

    BattleTurret _shooterTurret;
    protected BattleCreature _shooterCreature;
    protected BattleEntity _target;

    public void Shoot(BattleCreature shooter, BattleEntity target, float power)
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlaySFX(_shootSound, transform.position);
        _shooterCreature = shooter;
        _target = target;
        StartCoroutine(ShootCoroutine(shooter.Creature.AttackRange, target, power));
    }

    public void Shoot(BattleTurret shooter, BattleEntity target, float power)
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlaySFX(_shootSound, transform.position);
        _shooterTurret = shooter;
        _target = target;
        StartCoroutine(ShootCoroutine(shooter.Turret.GetCurrentUpgrade().Range, target, power));
    }

    IEnumerator ShootCoroutine(float range, BattleEntity target, float power)
    {
        float targetScale = transform.localScale.x;
        transform.localScale = transform.localScale * 0.5f;
        transform.DOScale(targetScale, 1f);
        transform.LookAt(target.transform);

        Vector3 startingPos = transform.position;

        Vector3 finalPos = target.Collider.bounds.center;

        Vector3 dir = (finalPos - startingPos).normalized;
        Vector3 destination = startingPos + dir * range;

        float t = 0;
        float step = (_speed / range) * Time.fixedDeltaTime;

        while (t <= 1.0f)
        {
            t += step;
            Vector3 newPos = Vector3.Lerp(startingPos, destination, t);
            transform.position = newPos;
            yield return new WaitForFixedUpdate();
        }

        yield return DestroySelf(target.Collider.bounds.center);
    }

    protected virtual IEnumerator HitTarget(BattleEntity target)
    {
        if (_shooterCreature != null) StartCoroutine(target.GetHit(_shooterCreature));
        if (_shooterTurret != null) StartCoroutine(target.GetHit(_shooterTurret));

        yield return DestroySelf(transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Tags.BattleObstacleLayer)
        {
            StopAllCoroutines();
            StartCoroutine(DestroySelf(transform.position));
            return;
        }

        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (!IsTargetValid(battleEntity)) return;

            StopAllCoroutines();
            StartCoroutine(HitTarget(battleEntity));
            return;
        }
    }

    bool IsTargetValid(BattleEntity battleEntity)
    {
        if (battleEntity.IsDead) return false;

        if (_shooterCreature != null)
            if (_shooterCreature.Team == battleEntity.Team) return false;
        // HERE: turret could have team too...

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
