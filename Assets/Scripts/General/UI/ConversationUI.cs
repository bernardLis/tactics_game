using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConversationUI : MonoBehaviour
{
	public UIDocument UIDocument;
	VisualElement conversationContainer;
	VisualElement conversationPortrait;
	Label conversationText;

	bool conversationOnGoing;
	float topPercent;
	IVisualElementScheduledItem scheduler;

	bool printTextCoroutineFinished = true;

	IEnumerator typeTextCoroutine;

	#region Singleton
	public static ConversationUI instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of ConversationUI found");
			return;
		}
		instance = this;

		#endregion

		// getting ui elements
		var root = UIDocument.rootVisualElement;
		conversationContainer = root.Q<VisualElement>("conversationContainer");
		conversationPortrait = root.Q<VisualElement>("conversationPortrait");
		conversationText = root.Q<Label>("conversationText");
	}

	public void ShowUI()
	{
		if (!conversationOnGoing)
		{
			conversationOnGoing = true;
			conversationContainer.style.display = DisplayStyle.Flex;

			// https://forum.unity.com/threads/animation-via-code-examples.948161/
			// https://forum.unity.com/threads/propertydrawer-with-uielements-changes-in-array-dont-refresh-inspector.747467/
			// https://docs.unity3d.com/ScriptReference/UIElements.IVisualElementScheduledItem.html
			// set the container all the way to the bottom
			topPercent = 100f;

			conversationContainer.style.top = Length.Percent(topPercent);

			//print("show UI after setting topPercent " + conversationContainer.style.top);
			// 'animate' it to come up 
			scheduler = conversationContainer.schedule.Execute(() => AnimateConversationBoxUp()).Every(10); // ms
		}
	}

	public void HideUI()
	{
		if (conversationOnGoing)
		{
			conversationOnGoing = false;
			scheduler = conversationContainer.schedule.Execute(() => AnimateConversationBoxDown()).Every(10); // ms
		}

	}

	void AnimateConversationBoxUp()
	{
		if (topPercent > 75f)
		{
			conversationContainer.style.top = Length.Percent(topPercent);
			topPercent--;
			return;
		}

		// TODO: idk how to destroy scheduler...
		scheduler.Pause();
	}

	void AnimateConversationBoxDown()
	{
		if (topPercent < 100f)
		{
			conversationContainer.style.top = Length.Percent(topPercent);
			topPercent++;
			return;
		}

		// TODO: idk how to destroy scheduler...
		scheduler.Pause();
		conversationContainer.style.display = DisplayStyle.None;
	}

	public void SetPortrait(Sprite sprite)
	{
		conversationPortrait.style.backgroundImage = new StyleBackground(sprite);
	}

	public void SetText(string text)
	{
		conversationText.style.color = Color.white;

		//StopAllCoroutines();
		if (typeTextCoroutine != null)
			StopCoroutine(typeTextCoroutine);

		typeTextCoroutine = TypeText(text);
		StartCoroutine(typeTextCoroutine);
		printTextCoroutineFinished = false;
	}

	public void SkipTextTyping(string text)
	{
		conversationText.style.color = Color.white;

		if (typeTextCoroutine != null)
			StopCoroutine(typeTextCoroutine);
		//StopAllCoroutines();
		conversationText.text = text;
		printTextCoroutineFinished = true;
	}

	public bool IsLinePrinted()
	{
		return printTextCoroutineFinished;
	}

	IEnumerator TypeText(string text)
	{
		conversationText.text = "";
		char[] charArray = text.ToCharArray();
		for (int i = 0; i < charArray.Length; i++)
		{
			conversationText.text += charArray[i];
			if (i == charArray.Length - 1)
				printTextCoroutineFinished = true;
			yield return new WaitForSeconds(0.03f);

		}
	}


}
