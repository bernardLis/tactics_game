using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    float _speed = 6f;

    Vector3 _adjustedTargetPosition;

    public virtual async Task<Transform> Shoot(Transform shooter, Vector3 targetPos)
    {
        // shoot from the chest area
        transform.position = transform.position + (Vector3.up * 0.5f);
        // shoot at the chest area
        _adjustedTargetPosition = targetPos + (Vector3.up * 0.5f);
        // look at the target;
        transform.right = -(_adjustedTargetPosition - transform.position); // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html

        // every frame move towards the target
        while (Vector2.Distance(_adjustedTargetPosition, transform.position) > 0.01f)
        {

            // Check whether we are hitting something
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (Collider2D c in cols)
                if (c.transform != shooter && !c.CompareTag(Tags.BoundCollider))
                {
                    HitSomething();
                    return c.transform;
                }

            float step = _speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, _adjustedTargetPosition, step);
            await Task.Yield();
        }
        // code should never get here
        DestroySelf();
        return null;
    }

    protected virtual void HitSomething()
    {
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
