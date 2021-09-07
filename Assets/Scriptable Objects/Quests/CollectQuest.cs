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
		qState = QuestState.ACTIVE;
	}

	public override void CompleteQuest()
	{
		// TODO: show ui
		Debug.Log("quest completed");

		qState = QuestState.COMPLETED;
	}

	public override void FailQuest()
	{
		// TODO: show ui
		Debug.Log("quest failed");

		qState = QuestState.FAILED;
	}

}
