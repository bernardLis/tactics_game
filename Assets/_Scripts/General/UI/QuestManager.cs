using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public Quest[] AllQuests;

    public List<Quest> ReturnActiveQuests()
    {
        List<Quest> activeQuests = new List<Quest>();
        foreach (Quest quest in AllQuests)
        {
            if (quest.State == QuestState.ACTIVE)
                activeQuests.Add(quest);
        }
        return activeQuests;
    }

    public List<Quest> ReturnCompletedQuests()
    {
        List<Quest> activeQuests = new List<Quest>();
        foreach (Quest quest in AllQuests)
        {
            if (quest.State == QuestState.COMPLETED)
                activeQuests.Add(quest);
        }
        return activeQuests;
    }

    public List<Quest> ReturnFailedQuests()
    {
        List<Quest> activeQuests = new List<Quest>();
        foreach (Quest quest in AllQuests)
        {
            if (quest.State == QuestState.FAILED)
                activeQuests.Add(quest);
        }
        return activeQuests;
    }
}
