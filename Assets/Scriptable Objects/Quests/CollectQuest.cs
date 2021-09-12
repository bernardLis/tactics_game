using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Collect Quest")]
public class CollectQuest : Quest
{
	public override void TriggerQuest()
	{
		// TODO: show ui
		Debug.Log("quest triggered");
		// initialize quest goals
		foreach (var qGoal in qGoals)
		{
			qGoal.Initialize();
		}

		GameUI.instance.DisplayLogText("New quest: " + qName);

		qState = QuestState.ACTIVE;
	}

	public override void CompleteQuest()
	{
		// TODO: show ui
		Debug.Log("quest completed");
		GameUI.instance.DisplayLogText("Quest completed: " + qName);


		qState = QuestState.COMPLETED;
	}

	public override void FailQuest()
	{
		// TODO: show ui
		Debug.Log("quest failed");
		GameUI.instance.DisplayLogText("Quest failed: " + qName);

		qState = QuestState.FAILED;
	}

}
