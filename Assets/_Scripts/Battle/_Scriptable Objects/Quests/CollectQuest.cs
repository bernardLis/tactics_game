using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Quests/Collect Quest")]
public class CollectQuest : Quest
{
	public override void Trigger()
	{
		// initialize quest goals
		foreach (var qGoal in Goals)
		{
			if (qGoal != null)
				qGoal.Initialize();
		}

		GameUI.Instance.DisplayLogText("New quest: " + Title);

		State = QuestState.ACTIVE;
	}

	public override void Complete()
	{
		State = QuestState.COMPLETED;

		// UI
		GameUI.Instance.DisplayLogText("Quest completed: " + Title);

		// remove quest items
		foreach (var qGoal in Goals)
		{
			qGoal.CleanUp();
		}

		// give player the reward
		InventoryManager.Instance.Add(Reward);
	}

	public override void Fail()
	{
		// TODO: show ui
		GameUI.Instance.DisplayLogText("Quest failed: " + Title);

		State = QuestState.FAILED;
	}

}
