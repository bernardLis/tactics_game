using UnityEngine;

public class PushableObstacle : Pushable
{
	BoxCollider2D objectCollider;

	int boulderDmg = 50;
	public override void IsPushed(Vector3 dir)
	{
		base.IsPushed(dir);
		Invoke("CollisionCheck", 0.35f);
	}

	protected override void CollisionCheck()
	{
		// check what is in character's new place and act accordingly
		objectCollider = transform.GetComponentInChildren<BoxCollider2D>();
		objectCollider.enabled = false;

		Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);

		if (col != null)
		{
			// player/enemy get dmged by 50 and boulder is destroyed
			// character colliders are children
			if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
			{
				targetStats = col.transform.parent.GetComponent<CharacterStats>();
				targetStats.TakePiercingDamage(boulderDmg);
				Destroy(gameObject);
			}
			// boulder is destroyed when it hits another boulder
			else if (col.transform.gameObject.CompareTag("Stone"))
			{
				Debug.Log("stone");
				Destroy(gameObject);
			}
			// boulder destroys traps
			else if (col.transform.gameObject.CompareTag("Trap"))
			{
				Destroy(col.transform.gameObject);
			}
		}
		// TODO: boulder is destroyed when it falls in the river and creates a walkable tile
		// currently you can't target it on the river bank
		if (objectCollider != null)
		{
			objectCollider.enabled = true;
		}
	}
}
