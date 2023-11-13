using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProjectileBoss : BattleProjectile
{
    BattleBoss _boss;
    float _time;
    int _power;
    public void Shoot(BattleBoss boss, Vector3 dir, float time, int power)
    {
        _boss = boss;
        _time = time;
        _power = power;
        StartCoroutine(ShootCoroutine(dir));
    }

    IEnumerator ShootCoroutine(Vector3 dir)
    {
        float t = 0;
        while (t <= _time)
        {
            transform.position += dir * _speed * Time.fixedDeltaTime;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return DestroySelf(transform.position);
    }

    protected override IEnumerator HitTarget(BattleEntity target)
    {
        GetComponent<SphereCollider>().enabled = false;
        target.BaseGetHit(_power, Color.black);

        yield return DestroySelf(transform.position);
    }


}
