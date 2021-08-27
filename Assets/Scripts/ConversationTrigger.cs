using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CircleCollider2D))]
public class ConversationTrigger : MonoBehaviour
{
	[SerializeField]
	UIDocument UIDocument;
	VisualElement tooltipUI;
	Label interactionTooltip;

	[SerializeField]
	Conversation conversation;
	// https://www.youtube.com/watch?v=_nRzoTzeyxU
	Queue<Line> lines;
	Line currentLine;

	[SerializeField]
	FMPlayerInteractionController interactionController;

	void Awake()
	{
		// getting ui elements
		var root = UIDocument.rootVisualElement;
		tooltipUI = root.Q<VisualElement>("tooltipUI");
		interactionTooltip = root.Q<Label>("interactionTooltip");
	}

	void Start()
	{
		lines = new Queue<Line>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("PlayerCollider"))
		{
			DisplayConversationTooltip();
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("PlayerCollider"))
		{
			HideConversationTooltip();
		}
	}

	void DisplayConversationTooltip()
	{
		tooltipUI.style.display = DisplayStyle.Flex;
		interactionTooltip.text = "'f' to talk";
		interactionTooltip.style.display = DisplayStyle.Flex;
		// TODO: resize tooltipUI to fit the text
		// width is resolved only on the next frame
	}

	void HideConversationTooltip()
	{
		tooltipUI.style.display = DisplayStyle.None;
		interactionTooltip.style.display = DisplayStyle.None;
	}

	public void StartConversation()
	{
		print("starting conversation");
		HideConversationTooltip();
		ConversationUI.instance.ShowUI();

		lines.Clear();

		foreach (Line line in conversation.cLines)
		{
			lines.Enqueue(line);
		}

		DisplayNextLine();
	}

	public void DisplayNextLine()
	{
		bool isLinePrinted = ConversationUI.instance.IsLinePrinted();

		// set-up to use one function for:
		// * showing new line of dialogue
		// * skip waiting for the line to be printed if someone is in hurry
		// * ending convo if there are no more lines
		if (isLinePrinted)
		{
			if (lines.Count == 0)
			{
				EndConversation();
				return;
			}
			currentLine = lines.Dequeue();
			ConversationUI.instance.SetText(currentLine.text);
			ConversationUI.instance.SetPortrait(currentLine.character.portrait);
		}
		else
		{
			ConversationUI.instance.SkipTextTyping(currentLine.text);
		}
	}

	public void EndConversation()
	{
		ConversationUI.instance.HideUI();
		// TODO: should I display tooltip and allow hero to talk again? 
		// or should i allow him to talk only when he goes out of the circle and then back
		//StartCoroutine(DelayedTooltipDisplay());
	}
	/*
	IEnumerator DelayedTooltipDisplay()
	{
		yield return new WaitForSeconds(1);
		// TODO: all this kinda sucks...
		DisplayConversationTooltip();
		interactionController.conversationStarted = false;
	}
	*/
}
