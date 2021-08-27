using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapInstance : MonoBehaviour
{
	public static TileMapInstance instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of TileMap found");
			return;
		}
		instance = this;
	}

}
