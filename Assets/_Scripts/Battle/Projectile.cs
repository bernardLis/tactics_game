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

    protected BattleCreature _shooter;
    protected BattleEntity _target;

    public void Shoot(BattleCreature shooter, BattleEntity target, float power)
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlaySFX(_shootSound, transform.position);
        _shooter = shooter;
        _target = target;
        StartCoroutine(ShootCoroutine(shooter, target, power));
    }

    IEnumerator ShootCoroutine(BattleCreature shooter, BattleEntity target, float power)
    {
        float targetScale = transform.localScale.x;
        transform.localScale = transform.localScale * 0.5f;
        transform.DOScale(targetScale, 1f);
        transform.LookAt(target.transform);

        Vector3 startingPos = transform.position;

        Vector3 finalPos = target.Collider.bounds.center;

        Vector3 dir = (finalPos - startingPos).normalized;
        Vector3 destination = startingPos + dir * shooter.Creature.AttackRange;

        float t = 0;
        float step = (_speed / _shooter.Creature.AttackRange) * Time.fixedDeltaTime;

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
        StartCoroutine(target.GetHit(_shooter));
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
            if (battleEntity.Team == _shooter.Team) return;
            if (battleEntity.IsDead) return;

            StopAllCoroutines();
            StartCoroutine(HitTarget(battleEntity));
            return;
        }
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
