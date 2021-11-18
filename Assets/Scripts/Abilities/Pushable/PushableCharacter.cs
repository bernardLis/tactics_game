using UnityEngine;

public class PushableCharacter : Pushable
{
	BoxCollider2D characterCollider;

	int characterDmg = 10;
	public override void IsPushed(Vector3 dir)
	{
		base.IsPushed(dir);
		Invoke("CollisionCheck", 0.35f);
	}

	protected override void CollisionCheck()
	{
		// check what is in boulders new place and act accordingly
		characterCollider = transform.GetComponentInChildren<BoxCollider2D>();
		characterCollider.enabled = false;

		Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);

		if (col != null)
		{
			// player/enemy get dmged by 10 and are moved back to their starting position
			// character colliders are children
			if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
			{
				myStats.TakePiercingDamage(characterDmg);

				targetStats = col.transform.parent.GetComponent<CharacterStats>();
				targetStats.TakePiercingDamage(characterDmg);
				// move back to starting position (if target is not dead)
				// TODO: test what happens when target dies
				if (targetStats.currentHealth > 0)
				{
					StartCoroutine(MoveToPosition(startingPos, 0.5f));
				}
			}
			// character destroys boulder when they are pushed into it + 10dmg to self
			else if (col.transform.gameObject.CompareTag("Stone"))
			{
				myStats.TakePiercingDamage(characterDmg);

				Destroy(col.transform.gameObject);
			}
			// character triggers traps
			else if (col.transform.gameObject.CompareTag("Trap"))
			{
				int dmg = col.transform.GetComponent<FootholdTrap>().damage;

				myStats.TakePiercingDamage(dmg);
				// movement range is down by 1 for each trap enemy walks on
				myStats.movementRange.AddModifier(-1);

				Destroy(col.transform.gameObject);
			}
			else
			{
				myStats.TakePiercingDamage(characterDmg);

				StartCoroutine(MoveToPosition(startingPos, 0.5f));
			}
		}
		// TODO: pushing characters into the river/other obstacles?
		// currently you can't target it on the river bank
		if (characterCollider != null)
		{
			characterCollider.enabled = true;
		}
	}
}
