using System.Collections;
using UnityEngine;

public class Pushable : MonoBehaviour
{
	protected Vector3 startingPos;
	protected Vector3 finalPos;

	protected CharacterStats myStats;
	protected CharacterStats targetStats;

	protected virtual void Awake()
	{
		myStats = GetComponent<CharacterStats>();
	}

	public virtual void IsPushed(Vector3 dir)
	{
		startingPos = transform.position;
		finalPos = transform.position + dir;

		StartCoroutine(MoveToPosition(finalPos, 0.5f));
	}

	protected virtual void CollisionCheck()
	{
		// this method is meant to be overwritten
	}

	protected IEnumerator MoveToPosition(Vector3 finalPos, float time)
	{
		Vector3 startingPos = transform.position;

		float elapsedTime = 0;

		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		UpdateAstar();
	}

	void UpdateAstar()
	{
		// TODO: is that alright? 
		// Recalculate all graphs
		AstarPath.active.Scan();
	}


}
