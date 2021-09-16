using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

	public Quest[] allQuests;

	void Awake()
	{
		foreach(Quest quest in allQuests)
		{
			// TODO: the state of the quest should be loaded from the save game, if it exists
			quest.qState = QuestState.INACTIVE;
		}
	}

	public List<Quest> ReturnActiveQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.ACTIVE)
				activeQuests.Add(quest);
		}
		return activeQuests;
	}

	public List<Quest> ReturnCompletedQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.COMPLETED)
				activeQuests.Add(quest);
		}
		return activeQuests;
	}

	public List<Quest> ReturnFailedQuests()
	{
		List<Quest> activeQuests = new List<Quest>();
		foreach (Quest quest in allQuests)
		{
			if (quest.qState == QuestState.FAILED)
				activeQuests.Add(quest);
		}
		return activeQuests;
	}

	public Quest ReturnQuestFromID(int id)
	{
		foreach (Quest quest in allQuests)
		{
			if (quest.qID == id)
				return quest;
		}
		return null;
	}
}
