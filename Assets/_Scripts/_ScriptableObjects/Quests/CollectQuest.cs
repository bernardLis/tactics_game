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

		State = QuestState.ACTIVE;
	}

	public override void Complete()
	{
		State = QuestState.COMPLETED;

		// remove quest items
		foreach (var qGoal in Goals)
		{
			qGoal.CleanUp();
		}

		// give player the reward
		GameManager.Instance.GetComponent<InventoryManager>().Add(Reward);
	}

	public override void Fail()
	{
		State = QuestState.FAILED;
	}

}
