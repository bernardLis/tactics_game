using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
	public static AudioScript instance;
	void Awake()
	{
		#region singleton

		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of AudioScript found");
			return;
		}
		instance = this;
		#endregion
	}
}
