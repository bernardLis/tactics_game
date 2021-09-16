using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManConversationTrigger : ConversationTrigger
{

	[SerializeField]
	RabbitSpawner domesticatedRabbitSpawner;


	public Conversation rabbitQuestSO;
	public Conversation noRabbitsSO;
	public Conversation hasRabbitsSO;
	public Conversation afterRabbitQuestCompletedSO;

	// TODO: does this make sense?
	// if I don't instatiate them, cseen is persistent between starts of the new game
	// it is a great way to save progress btw. - after a quick google, apparently, it is not a good way to save progress.
	Conversation rabbitQuest;
	Conversation noRabbits;
	Conversation hasRabbits;
	Conversation afterRabbitQuestCompleted;

	bool spawnedRabbits;

	QuestManager questManager;
	Quest rabbitQ;

	protected override void Start()
	{
		base.Start();


		questManager = GameManager.instance.GetComponent<QuestManager>();

		rabbitQuest = Instantiate(rabbitQuestSO);
		noRabbits = Instantiate(noRabbitsSO);
		hasRabbits = Instantiate(hasRabbitsSO);
		afterRabbitQuestCompleted = Instantiate(afterRabbitQuestCompletedSO);

		rabbitQ = questManager.ReturnQuestFromID(0);
	}

	protected override void SetCurrentConversation()
	{
		if (!rabbitQuest.cSeen)
		{
			currentConversation = rabbitQuest;
		}
		// TODO: check if **all** goals of rabbit quest were completed 
		// TODO: does this make sense to display different convo?
		// TODO: I am not certain about all this.. there must be a better way
		else if (rabbitQ.qGoals[0].qGoalState == QuestGoalState.ACTIVE)
		{
			currentConversation = noRabbits;
		}
		else if (rabbitQ.qGoals[0].qGoalState == QuestGoalState.COMPLETED && rabbitQ.qState != QuestState.COMPLETED)
		{
			currentConversation = hasRabbits;
		}
		else if(rabbitQ.qState == QuestState.COMPLETED)
		{
			currentConversation = afterRabbitQuestCompleted;
		}
	}

	public override void EndConversation()
	{
		base.EndConversation();

		if (rabbitQ.qGoals[0].qGoalState == QuestGoalState.COMPLETED && !spawnedRabbits)
		{
			spawnedRabbits = true;
			domesticatedRabbitSpawner.SpawnRabbit();
			domesticatedRabbitSpawner.SpawnRabbit();
			domesticatedRabbitSpawner.SpawnRabbit();
		}
	}
}
