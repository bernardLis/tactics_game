using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Conversations/Conversation")]
public class Conversation : BaseScriptableObject
{
	public bool Seen = false;
	public Line[] Lines;

	// conversations trigger a change in a quest
	[Header("Quests")]
	public Quest Quest;
	public bool QuestTrigger;
	public bool QuestCompleted; // TODO: these 3 could be an enum
	public bool QuestFailed;

	public void EndConversation()
	{
		Seen = true;
		if (Quest != null)
			ChangeQuestStatus();
	}

	public void ChangeQuestStatus()
	{
		if (QuestTrigger)
			Quest.Trigger();
		if (QuestCompleted)
			Quest.Complete();
		if (QuestFailed)
			Quest.Fail();
	}
}
