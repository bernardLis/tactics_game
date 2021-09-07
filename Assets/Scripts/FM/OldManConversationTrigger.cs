using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManConversationTrigger : ConversationTrigger
{

	[SerializeField]
	RabbitSpawner domesticatedRabbitSpawner;

	Transform player;

	// TODO: rabbit check should be done via inventory system - or something even smarter.
	FMPlayerInteractionController playerInteractionController;

	public Conversation rabbitQuestSO;
	public Conversation noRabbitsSO;
	public Conversation hasRabbitsSO;
	public Conversation afterRabbitQuestCompletedSO;


	// TODO: does this make sense?
	// if I don't instatiate them, cseen is persistent between starts of the new game
	// it is a great way to save progress btw. 
	Conversation rabbitQuest;
	Conversation noRabbits;
	Conversation hasRabbits;
	Conversation afterRabbitQuestCompleted;

	bool spawnedRabbits;


	protected override void Start()
	{
		base.Start();

		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerInteractionController = player.GetComponent<FMPlayerInteractionController>();

		rabbitQuest = Instantiate(rabbitQuestSO);
		noRabbits = Instantiate(noRabbitsSO);
		hasRabbits = Instantiate(hasRabbitsSO);
		afterRabbitQuestCompleted = Instantiate(afterRabbitQuestCompletedSO);
	}


	protected override void SetCurrentConversation()
	{
		if (!rabbitQuest.cSeen)
		{
			currentConversation = rabbitQuest;
		}
		else if (playerInteractionController.rabbitsCaught < 3)
		{
			currentConversation = noRabbits;
		}
		else if (playerInteractionController.rabbitsCaught >= 3 && !hasRabbits.cSeen)
		{
			currentConversation = hasRabbits;
		}
		else if(hasRabbits.cSeen)
		{
			currentConversation = afterRabbitQuestCompleted;
		}
	}

	public override void EndConversation()
	{
		base.EndConversation();
		if (currentConversation == hasRabbits && !spawnedRabbits)
		{
			spawnedRabbits = true;
			domesticatedRabbitSpawner.SpawnRabbit();
			domesticatedRabbitSpawner.SpawnRabbit();
			domesticatedRabbitSpawner.SpawnRabbit();
		}
	}
}
