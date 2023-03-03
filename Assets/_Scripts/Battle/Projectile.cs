using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Sound _explosionSound;
    [SerializeField] Sound _shootSound;

    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _explosion;
    BattleEntity _target;

    public void Shoot(BattleEntity target, float speed, float power)
    {
        if (Random.value > 0.5f)
            AudioManager.Instance.PlaySFX(_shootSound, transform.position);
        _target = target;
        StartCoroutine(ShootCoroutine(target, speed, power));
    }
    IEnumerator ShootCoroutine(BattleEntity target, float speed, float power)
    {

        //https://gamedev.stackexchange.com/questions/100535/coroutine-to-move-to-position-passing-the-movement-speed
        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.GFX.transform.position;
        float t = 0;
        float step = (speed / (startingPos - finalPos).magnitude) * Time.fixedDeltaTime;

        while (t <= 1.0f)
        {
            if (target != null)
                finalPos = target.GFX.transform.position;
            t += step;
            transform.position = Vector3.Lerp(startingPos, finalPos, t);

            yield return new WaitForFixedUpdate();
        }
        _gfx.SetActive(false);
        if (Random.value > 0.5f)
            AudioManager.Instance.PlaySFX(_explosionSound, transform.position);
        _explosion.SetActive(true);

        if (target != null)
        {
            transform.position = target.GFX.transform.position;
            yield return target.GetHit(power);
        }

        yield return DestroySelf();
    }

    public IEnumerator DestroySelf()
    {
        // explosion on hit
        // _gfx.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}
