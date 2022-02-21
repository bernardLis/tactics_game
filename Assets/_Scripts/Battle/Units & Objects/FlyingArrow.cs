using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingArrow : MonoBehaviour, IShootable<Transform>
{
	[Header("Attributes")]
	public float speed;

	Transform target;
	Vector3 adjustedTargetPosition;


    public void Shoot(Transform _target)
    {
		target = _target;

		// shoot from the chest area
		transform.position = transform.position + (Vector3.up * 0.5f);
		// shoot at the chest area
		adjustedTargetPosition = _target.position + (Vector3.up * 0.5f);
	}

	// Update is called once per frame
	void Update()
	{
		if(target == null)
        {
			return;
        }

		// look at the target;
		transform.right = -(adjustedTargetPosition - transform.position); // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html

		// move towards the target
		float step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, adjustedTargetPosition, step);
		
		if (Vector2.Distance(adjustedTargetPosition, transform.position) < 0.01f)
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
