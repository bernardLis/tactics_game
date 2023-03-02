using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _explosion;
    BattleEntity _target;

    public void Shoot(BattleEntity target, float speed, float power)
    {
        _target = target;
        StartCoroutine(ShootCoroutine(target, speed, power));
    }
    IEnumerator ShootCoroutine(BattleEntity target, float speed, float power)
    {

        //https://gamedev.stackexchange.com/questions/100535/coroutine-to-move-to-position-passing-the-movement-speed
        Vector3 startingPos = transform.position;
        Vector3 finalPos = target.transform.position;
        float t = 0;
        float step = (speed / (startingPos - finalPos).magnitude) * Time.fixedDeltaTime;

        while (t <= 1.0f)
        {
            if (target != null)
                finalPos = target.transform.position;
            t += step;
            transform.position = Vector3.Lerp(startingPos, finalPos, t);
            yield return new WaitForFixedUpdate();
        }

        if (target != null)
        {
            transform.position = target.transform.position;
            yield return target.GetHit(power);
        }

        yield return DestroySelf();
    }

    public IEnumerator DestroySelf()
    {
        // explosion on hit
        _gfx.SetActive(false);
        _explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}
