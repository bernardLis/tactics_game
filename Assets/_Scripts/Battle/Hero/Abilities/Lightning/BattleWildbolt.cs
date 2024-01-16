using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class BattleWildbolt : BattleProjectile
{
    public void Fire(Ability ability)
    {
        _ability = ability;
        EnableProjectile();
        StartCoroutine(FireCoroutine());
    }

    public IEnumerator FireCoroutine()
    {
        float endTime = Time.time + _ability.GetDuration();
        while (Time.time < endTime)
        {
            // choose a random direction but keep y = 0
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            transform.rotation = Quaternion.LookRotation(dir);

            _speed = Random.Range(5f, 15f);
            yield return GoForward(dir, Random.Range(0.5f, 2f));
        }
        yield return Explode(transform.position);
    }

    IEnumerator GoForward(Vector3 dir, float timeInSeconds)
    {
        float t = 0;
        while (t <= timeInSeconds)
        {
            transform.position += _speed * Time.fixedDeltaTime * dir;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

}
