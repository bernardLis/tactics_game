using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public Quest[] allQuests;

	public List<Quest> ReturnActiveQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.ACTIVE)
			{
				activeQuests.Add(quest);
			}
		}
		return activeQuests;
	}

	public List<Quest> ReturnCompletedQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.COMPLETED)
			{
				activeQuests.Add(quest);
			}
		}
		return activeQuests;
	}

	public List<Quest> ReturnFailedQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.FAILED)
			{
				activeQuests.Add(quest);
			}
		}
		return activeQuests;
	}
}
