using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMPlayerInteractionController : MonoBehaviour
{
	public InputMaster controls;
	ConversationTrigger conversationTrigger;
	public bool conversationOngoing;

	void Awake()
	{
		controls = new InputMaster();
		controls.FMPlayer.Interact.performed += ctx => Interact();
	}

	void OnEnable()
	{
		controls.Enable();
	}

	void OnDisable()
	{
		controls.Disable();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("ConversationCollider"))
		{
			conversationTrigger = col.GetComponent<ConversationTrigger>();
		}

	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("ConversationCollider"))
		{
			// reseting 
			conversationOngoing = false;

			conversationTrigger.EndConversation();
			conversationTrigger.HideConversationTooltip();
			conversationTrigger = null;
		}
	}

	void Interact()
	{
		if (conversationTrigger != null)
		{
			if (!conversationOngoing)
			{
				conversationOngoing = true;

				conversationTrigger.StartConversation();
				return;
			}
			// I need to know whether the line stopped printing = yes > start printing next line, no print the full line without the typing effect
			// I need to know when there are not more lines and convo ends = reset bool, show tooltip again
			conversationTrigger.DisplayNextLine();
		}
	}


}
