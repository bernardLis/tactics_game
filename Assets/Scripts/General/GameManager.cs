using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	void Awake()
	{
		// singleton
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
	}

}
