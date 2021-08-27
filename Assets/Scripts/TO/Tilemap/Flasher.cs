using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
	Material mat;

	void Awake()
	{
		mat = GetComponent<Renderer>().material;
	}

	public void StartFlashing(Color col)
	{
		StartCoroutine(Flash(col));
	}

	IEnumerator Flash(Color col)
	{
		mat.color = col;

		float flashTimer = 0f;
		while (true)
		{
			// *2 coz I want it to flash quicker 
			// +0.2f coz I don't want it to disapear completly
			float a = 1.3f - Mathf.PingPong(flashTimer / 2f, 1f);
			a = Mathf.Clamp(a, 0f, 1f);
			col.a = a;
			mat.color = col;
			//mat.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(flashTimer, 1f));
			flashTimer += Time.deltaTime;
			yield return null;
		}
	}

}
