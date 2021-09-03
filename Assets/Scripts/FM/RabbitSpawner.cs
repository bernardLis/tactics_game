using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject rabbitPrefab;
	[SerializeField]
	GameObject poofEffect;

	int startingRabbits = 3;

	void Start()
	{
		// spawn rabbits at the beginnig of the game
		for (var i = 0; i < startingRabbits; i++)
		{
			StartCoroutine(SpawnRabbit());
		}
	}

	IEnumerator SpawnRabbit()
	{
		Vector3 spawnPosition = ChooseSpawnPosition();

		// when rabbit "hides" it sends an event to spawner and spawner spawns a new rabbit
		Destroy(Instantiate(poofEffect, spawnPosition, Quaternion.identity), 1f);

		yield return new WaitForSeconds(0.5f);

		GameObject rabbit = Instantiate(rabbitPrefab, spawnPosition, Quaternion.identity);
		rabbit.transform.parent = transform;
		rabbit.GetComponent<WildRabbit>().rabbitHides += OnRabbitHides;
	}

	Vector3 ChooseSpawnPosition()
	{
		// rabbits should be spawned in a selected by me area
		float minX = -17.5f;
		float maxX = 4;
		float minY = 5.5f;
		float maxY = 23.5f;

		float x = Random.Range(minX, maxX);
		float y = Random.Range(minY, maxY);

		Vector3 spawnPosition = new Vector3(x, y, 0f);

		// rabbits cannot be spawn on obstacle tiles or the tile player is currently on
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
		if (hitColliders.Length != 0)
		{
			// start over;
			ChooseSpawnPosition();
		}

		return spawnPosition;
	}

	void OnRabbitHides()
	{
		StartCoroutine(SpawnRabbit());
	}
}
