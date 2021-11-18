using UnityEngine;

public class PushableTrap : Pushable
{
	BoxCollider2D objectCollider;
	FootholdTrap footholdTrap;

	protected override void Awake()
	{
		base.Awake();
		footholdTrap = GetComponent<FootholdTrap>();
	}

	public override void IsPushed(Vector3 dir)
	{
		base.IsPushed(dir);
		footholdTrap.isPushed = true;

		Invoke("CollisionCheck", 0.35f);
		// TODO: reset should be after trap stops moving and not at 1s randomly...
		Invoke("ResetPushed", 1f);
	}

	protected override void CollisionCheck()
	{
		// check what is in character's new place and act accordingly
		objectCollider = transform.GetComponentInChildren<BoxCollider2D>();
		objectCollider.enabled = false;

		Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);

		int dmg = GetComponent<FootholdTrap>().damage;

		if (col != null)
		{
			// trap is triggered on player/enemy
			// character colliders are children
			// enemy triggers trap from trap script
			if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
			{
				targetStats = col.transform.parent.GetComponent<CharacterStats>();

				targetStats.TakePiercingDamage(dmg);
				// movement range is down by 1 for each trap enemy walks on
				targetStats.movementRange.AddModifier(-1);

				Destroy(gameObject);
			}
			// trap is destroyed when it hits a boulder
			else if (col.transform.gameObject.CompareTag("Stone"))
				Destroy(gameObject);
			// one trap is destroyed when it hits another traps
			else if (col.transform.gameObject.CompareTag("Trap"))
				Destroy(gameObject);
		}

		// currently you can't target it on the river bank
		if (objectCollider != null)
			objectCollider.enabled = true;

		// reset pushed in the end;
	}

	void ResetPushed()
	{
		if (footholdTrap != null)
			footholdTrap.isPushed = false;
	}


}
