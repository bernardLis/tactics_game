using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Collect Quest")]
public class CollectQuest : Quest
{
	public override void Trigger()
	{
		// initialize quest goals
		foreach (var qGoal in qGoals)
		{
			if (qGoal != null)
				qGoal.Initialize();
		}

		GameUI.instance.DisplayLogText("New quest: " + qName);

		qState = QuestState.ACTIVE;
	}

	public override void Complete()
	{
		qState = QuestState.COMPLETED;

		// UI
		GameUI.instance.DisplayLogText("Quest completed: " + qName);

		// remove quest items
		foreach (var qGoal in qGoals)
		{
			qGoal.CleanUp();
		}

		// give player the reward
		Inventory.instance.Add(qReward);
	}

	public override void Fail()
	{
		// TODO: show ui
		GameUI.instance.DisplayLogText("Quest failed: " + qName);

		qState = QuestState.FAILED;
	}

}
