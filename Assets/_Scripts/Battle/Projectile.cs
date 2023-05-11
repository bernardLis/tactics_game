using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] Sound _explosionSound;
    [SerializeField] Sound _shootSound;

    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _explosion;
    BattleEntity _target;

    public void Shoot(BattleEntity shooter, BattleEntity target, float speed, float power)
    {
        AudioManager.Instance.PlaySFX(_shootSound, transform.position);
        _target = target;
        StartCoroutine(ShootCoroutine(shooter, target, speed, power));
    }

    IEnumerator ShootCoroutine(BattleEntity shooter, BattleEntity target, float speed, float power)
    {
        float targetScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, 2f);

        //https://gamedev.stackexchange.com/questions/100535/coroutine-to-move-to-position-passing-the-movement-speed
        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.Collider.bounds.center;
        float t = 0;
        float step = (speed / (startingPos - finalPos).magnitude) * Time.fixedDeltaTime;

        while (t <= 1.0f)
        {
            if (target != null) finalPos = target.Collider.bounds.center;
            t += step;
            Vector3 pos = Vector3.Lerp(startingPos, finalPos, t);
            //  if (pos.y < 1) pos.y = 1; // TODO: shitty fix for projectiles going under the ground
            transform.position = pos;
            transform.LookAt(target.transform);

            yield return new WaitForFixedUpdate();
        }

        if (target != null)
            StartCoroutine(target.GetHit(shooter));// yield return target.GetHit(shooter);

        yield return DestroySelf(target.Collider.bounds.center);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != Tags.BattleObstacleLayer)
            return;
        StopAllCoroutines();
        StartCoroutine(DestroySelf(transform.position));
    }

    public IEnumerator DestroySelf(Vector3 position)
    {
        _gfx.SetActive(false);
        AudioManager.Instance.PlaySFX(_explosionSound, position);
        _explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        transform.DOKill(transform);
        Destroy(gameObject);
    }
}
