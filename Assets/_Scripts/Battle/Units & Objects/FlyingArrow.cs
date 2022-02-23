using UnityEngine;

public class FlyingArrow : MonoBehaviour, IShootable<Transform>
{
    [Header("Attributes")]
    public float Speed = 10f;

    Transform _target;
    Vector3 _adjustedTargetPosition;


    public void Shoot(Transform _target)
    {
        this._target = _target;

        // shoot from the chest area
        transform.position = transform.position + (Vector3.up * 0.5f);
        // shoot at the chest area
        _adjustedTargetPosition = _target.position + (Vector3.up * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
            return;

        // look at the target;
        transform.right = -(_adjustedTargetPosition - transform.position); // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html

        // move towards the target
        float step = Speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _adjustedTargetPosition, step);

        if (Vector2.Distance(_adjustedTargetPosition, transform.position) < 0.01f)
        {
            HitTarget();
            return;
        }
    }

    public void HitTarget()
    {
        Destroy(gameObject);
    }
}
