using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	GameObject player;
	PlayerInput playerInput;

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

		player = GameObject.FindGameObjectWithTag("Player");
		playerInput = player.GetComponent<PlayerInput>();
	}

	public void EnableFMPlayerControls()
	{
		playerInput.SwitchCurrentActionMap("FMPlayer");
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
