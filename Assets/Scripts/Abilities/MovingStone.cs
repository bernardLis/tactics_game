using UnityEngine;

public class MovingStone : MonoBehaviour
{
	BoxCollider2D myCollider;
	public int boulderDmg = 50;
	CharacterStats stats;

	public void BoulderCheck(Vector3 finalPos)
	{
		Debug.Log("in moviong stone");
		// check what is in boulders new place and act accordingly
		myCollider = transform.GetComponent<BoxCollider2D>();
		myCollider.enabled = false;

		Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);

		if (col != null)
		{
			// player/enemy get dmged by 50 and boulder is destroyed
			// character colliders are children
			if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
			{
				Debug.Log("hitting a character");
				stats = col.transform.parent.GetComponent<CharacterStats>();
				stats.TakeDamage(boulderDmg);
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
				Debug.Log("trap");

				Destroy(col.transform.gameObject);
			}
		}
		// TODO: boulder is destroyed when it falls in the river
		// currently you can't target it on the river bank
		if (myCollider != null)
		{
			myCollider.enabled = true;
		}
	}


}
