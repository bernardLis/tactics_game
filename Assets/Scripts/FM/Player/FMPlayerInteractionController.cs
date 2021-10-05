using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FMPlayerInteractionController : MonoBehaviour
{
	PlayerInput playerInput;

	ConversationTrigger conversationTrigger;
	public bool conversationOngoing;

	void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
	}

	void OnEnable()
	{
		playerInput.actions["EnableQuestUI"].performed += ctx => GameUI.instance.GetComponent<QuestUI>().EnableQuestUI();
		playerInput.actions["EnableInventoryUI"].performed += ctx => GameUI.instance.GetComponent<InventoryUI>().EnableInventoryUI();

		playerInput.actions["Interact"].performed += ctx => Interact();
		// TODO: does it make sense?
		playerInput.actions["ConversationInteract"].performed += ctx => Interact();
	}

	void OnDisable()
	{
		playerInput.actions["Interact"].performed -= ctx => Interact();
		playerInput.actions["ConversationInteract"].performed -= ctx => Interact();
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

			//conversationTrigger.EndConversation();
			conversationTrigger.HideConversationTooltip();
			conversationTrigger = null;
		}
	}

	void Interact()
	{
		Debug.Log("interact is called");

		if (conversationTrigger != null)
		{
			if (!conversationOngoing)
			{
				conversationOngoing = true;
				Debug.Log("interact after all checks");

				// TODO: does it make sense?
				playerInput.SwitchCurrentActionMap("Conversation");

				conversationTrigger.StartConversation();
				return;
			}
			conversationTrigger.DisplayNextLine();
		}
	}


}
