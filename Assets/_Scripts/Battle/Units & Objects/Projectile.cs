using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float _speed = 5f;

    Vector3 _adjustedTargetPosition;
    Transform _shooter;

    [SerializeField] GameObject _impactEffect;
    [SerializeField] float _impactDuration = 1f;


    public virtual async Task<Transform> Shoot(Transform shooter, Vector3 targetPos)
    {
        _shooter = shooter;
        // spawn projectile in the tile in the direction towards the target
        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        Vector3 spawnLocation = transform.position + dirToTarget;
        transform.position = spawnLocation;

        _adjustedTargetPosition = targetPos;
        Debug.DrawLine(spawnLocation, _adjustedTargetPosition, Color.magenta, 3f);
        // look at the target;
        transform.right = -(_adjustedTargetPosition - transform.position); // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html

        Transform hit = CheckCollisions();
        if (hit != null)
        {
            HitSomething();
            return hit;
        }

        // every frame move towards the target
        while (Vector2.Distance(_adjustedTargetPosition, transform.position) > 0.01f)
        {
            hit = CheckCollisions();
            if (hit != null)
            {
                HitSomething();
                return hit;
            }

            float step = _speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, _adjustedTargetPosition, step);
            await Task.Yield();
        }

        DestroySelf();
        return null;
    }

    Transform CheckCollisions()
    {
        // Check whether we are hitting something
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (Collider2D c in cols)
        {
            if (c.transform != _shooter && (c.CompareTag(Tags.Obstacle)
            || c.CompareTag(Tags.Player) || c.CompareTag(Tags.Enemy)
            || c.CompareTag(Tags.PushableObstacle)))
            {
                return c.transform;
            }
        }
        return null;
    }

    protected virtual void HitSomething()
    {
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(Instantiate(_impactEffect, transform.position, Quaternion.identity), _impactDuration);
        Destroy(gameObject);
    }
}
