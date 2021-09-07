using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject rabbitPrefab, poofEffect;

	// rabbits should be spawned in a selected area
	[SerializeField]
	float minX, maxX, minY, maxY;

	[SerializeField]
	int startingRabbits = 3;

	Rabbit rabbitScript;

	void Start()
	{
		// spawn rabbits at the beginnig of the game
		for (var i = 0; i < startingRabbits; i++)
		{
			StartCoroutine(SpawnRabbitCoroutine());
		}
	}
	public void SpawnRabbit()
	{
		StartCoroutine(SpawnRabbitCoroutine());
	}

	IEnumerator SpawnRabbitCoroutine()
	{
		Vector3 spawnPosition = ChooseSpawnPosition();

		// when rabbit "hides" it sends an event to spawner and spawner spawns a new rabbit
		Destroy(Instantiate(poofEffect, spawnPosition, Quaternion.identity), 1f);

		yield return new WaitForSeconds(0.5f);

		GameObject rabbit = Instantiate(rabbitPrefab, spawnPosition, Quaternion.identity);
		rabbit.transform.parent = transform;
		rabbit.GetComponent<Rabbit>().rabbitSpawner = this;
		//rabbit.GetComponent<WildRabbit>().rabbitHides += OnRabbitHides;
	}

	Vector3 ChooseSpawnPosition()
	{

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

	/*
	void OnRabbitHides()
	{
		StartCoroutine(SpawnRabbitCoroutine());
	}
	*/
}
