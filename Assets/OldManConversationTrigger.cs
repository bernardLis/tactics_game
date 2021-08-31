using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManConversationTrigger : ConversationTrigger
{
	public Conversation rabbitQuestSO;
	public Conversation noRabbitsSO;
	public Conversation hasRabbitsSO;


	// TODO: does this make sense?
	// if I don't instatiate them, cseen is persistent between starts of the new game
	// it is a great way to save progress btw. 
	Conversation rabbitQuest;
	Conversation noRabbits;
	Conversation hasRabbits;


	protected override void Start()
	{
		base.Start();

		rabbitQuest = Instantiate(rabbitQuestSO);
		noRabbits = Instantiate(noRabbitsSO);
		hasRabbits = Instantiate(hasRabbitsSO);
	}


	protected override void SetCurrentConversation()
	{
		// meant to be overwritten
		if (!rabbitQuest.cSeen)
		{
			currentConversation = rabbitQuest;
		}
		else
		{
			currentConversation = noRabbits;
		}
	}

}
