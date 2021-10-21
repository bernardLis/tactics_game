using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public GameObject[] enemies;
	public GameObject[] playerCharacters;
	EnemyAI enemyAI;
	IEnumerator currentEnemyAI;

	#region Singleton
	public static EnemyManager instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Enemy Manager found");
			return;
		}
		instance = this;

		FindObjectOfType<TurnManager>().playerTurnEndEvent += OnPlayerTurnEnd;
	}
	#endregion

	void OnPlayerTurnEnd()
	{
		StartCoroutine(ForEachEnemy());
	}

	IEnumerator ForEachEnemy()
	{
		yield return new WaitForSeconds(1.5f);
		enemies = GameObject.FindGameObjectsWithTag("Enemy");

		// TODO: THIS IS A BAD IDEA 
		// but it fixes a problem where enemies did not have updated graph
		// after player moved stone at the end of player turn
		// Recalculate all graphs
		AstarPath.active.Scan();

		// for every enemy character
		foreach (var enemy in enemies)
		{
			if (enemy != null)
			{
				BasicCameraFollow.instance.followTarget = enemy.transform;
				enemyAI = enemy.GetComponent<EnemyAI>();
				// this waits until the previous corutine is done
				yield return StartCoroutine(enemyAI.RunAI());
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
