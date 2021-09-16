using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;

public class TurnDisplayer : MonoBehaviour
{
	UIDocument UIDocument;
	VisualElement turnTextContainer;
	Label turnText;

	Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

	public int turnCount = 0;
	void Awake()
	{
		UIDocument = GameUI.instance.GetComponent<UIDocument>();
		
		// getting ui elements
		var rootVisualElement = UIDocument.rootVisualElement;
		turnTextContainer = rootVisualElement.Q<VisualElement>("turnTextContainer");
		turnText = rootVisualElement.Q<Label>("turnText");

		// subscribing to Actions
		FindObjectOfType<TurnManager>().playerTurnEndEvent += OnPlayerTurnEnd;
		FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnEnemyTurnEnd;

		turnText.text = "TURN " + turnCount.ToString() + " - PLAYER";
	}

	void Start()
	{
		// coroutine queue
		StartCoroutine(CoroutineCoordinator());
		coroutineQueue.Enqueue(DisplayTurnText(false));
	}

	// https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
	// coroutine queue
	// TODO: is it performance-expensive? 
	IEnumerator CoroutineCoordinator()
	{
		while (true)
		{
			while (coroutineQueue.Count > 0)
				yield return StartCoroutine(coroutineQueue.Dequeue());
			yield return null;
		}
	}

	public void OnEnemyTurnEnd()
	{
		coroutineQueue.Enqueue(DisplayTurnText(false));
	}
	public void OnPlayerTurnEnd()
	{
		coroutineQueue.Enqueue(DisplayTurnText(true));
	}

	IEnumerator DisplayTurnText(bool playerTurnEnd)
	{
		if (playerTurnEnd)
		{
			turnText.text = "TURN " + turnCount.ToString() + " - ENEMY";
		}
		else
		{
			turnCount++;
			turnText.text = "TURN " + turnCount.ToString() + " - PLAYER";
		}

		turnTextContainer.style.display = DisplayStyle.Flex;

		// fade in
		float waitTime = 0;
		float fadeTime = 1f;
		while (waitTime < 1)
		{
			turnTextContainer.style.opacity = waitTime;
			yield return null;
			waitTime += Time.deltaTime / fadeTime;
		}

		yield return new WaitForSeconds(1);

		// fade out
		waitTime = 0;
		fadeTime = 1f;
		while (waitTime < 1)
		{
			turnTextContainer.style.opacity = 1 - waitTime;
			yield return null;
			waitTime += Time.deltaTime / fadeTime;
		}

		turnTextContainer.style.display = DisplayStyle.None;
	}
}
