using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CircleCollider2D))]
public class ConversationTrigger : MonoBehaviour
{
	UIDocument UIDocument;
	ConversationUI conversationUI;

	VisualElement tooltipUI;
	VisualElement interactionTooltipWrapper;
	Label interactionTooltipText;

	protected Conversation currentConversation;

	// https://www.youtube.com/watch?v=_nRzoTzeyxU
	Queue<Line> lines;
	Line currentLine;

	// player
	FMPlayerInteractionController playerInteractionController;

	protected virtual void Start()
	{
		UIDocument = GameUI.instance.GetComponent<UIDocument>();
		conversationUI = GameUI.instance.GetComponent<ConversationUI>();

		// getting ui elements
		var root = UIDocument.rootVisualElement;
		tooltipUI = root.Q<VisualElement>("tooltipUI");
		interactionTooltipWrapper = root.Q<VisualElement>("interactionTooltipWrapper");
		interactionTooltipText = root.Q<Label>("interactionTooltipText");

		lines = new Queue<Line>();

		if(GameObject.FindGameObjectWithTag("Player") != null)
			playerInteractionController = GameObject.FindGameObjectWithTag("Player").GetComponent<FMPlayerInteractionController>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("PlayerCollider"))
		{
			SetCurrentConversation();
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

	protected void DisplayConversationTooltip()
	{
		tooltipUI.style.display = DisplayStyle.Flex;
		interactionTooltipWrapper.style.display = DisplayStyle.Flex;
		interactionTooltipText.text = " to talk";
		// TODO: resize tooltipUI to fit the text
		// width is resolved only on the next frame
	}

	void HideConversationTooltip()
	{
		tooltipUI.style.display = DisplayStyle.None;
		interactionTooltipWrapper.style.display = DisplayStyle.None;
	}

	public void StartConversation()
	{
		HideConversationTooltip();
		conversationUI.ShowUI();

		lines.Clear();
		foreach (Line line in currentConversation.cLines)
		{
			lines.Enqueue(line);
		}

		DisplayNextLine();
	}

	public void DisplayNextLine()
	{
		bool isLinePrinted = conversationUI.IsLinePrinted();

		// set-up to use one function for:
		// * showing new line of dialogue
		// * skip waiting for the line to be printed if someone is in hurry
		// * ending convo if there are no more lines
		if (isLinePrinted)
		{
			if (lines.Count == 0)
			{
				// mark conversation as seen
				currentConversation.EndConversation();

				EndConversation();
				return;
			}
			currentLine = lines.Dequeue();

			if (currentLine != null)
			{
				conversationUI.SetText(currentLine.text);
				conversationUI.SetPortrait(currentLine.character.portrait);
			}
		}
		else
		{
			conversationUI.SkipTextTyping(currentLine.text);
		}
	}

	public virtual void EndConversation()
	{
		// TODO: should I display tooltip
		// TODO: should I display tooltip and allow hero to talk again? 
		// or should i allow him to talk only when he goes out of the circle and then back

		conversationUI.HideUI();

		SetCurrentConversation();
		DisplayConversationTooltip();

		// TODO: dunno if that's a correct way to handle this
		if (playerInteractionController != null)
			playerInteractionController.conversationOngoing = false;
	}

	protected virtual void SetCurrentConversation()
	{
		// meant to be overwritten
	}
}
