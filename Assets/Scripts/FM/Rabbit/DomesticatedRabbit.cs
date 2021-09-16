using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomesticatedRabbit : Rabbit
{

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		// Rabbit moves randomly from time to time
		if (Time.time > nextRandomMove)
		{
			RandomMove();
		}
	}
}
