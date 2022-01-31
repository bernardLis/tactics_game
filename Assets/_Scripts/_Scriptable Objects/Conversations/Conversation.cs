using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Conversations/Conversation")]
public class Conversation : BaseScriptableObject
{
	public string cId = "New Id";
	public bool cSeen = false;
	public string cName = "New Conversation";
	public string cDescription = "New Description";
	public Line[] cLines;


	// conversations trigger a change in a quest
	[Header("Quests")]
	public Quest quest;
	public bool questTrigger;
	public bool questCompleted;
	public bool questFailed;

	public void EndConversation()
	{
		cSeen = true;
		if (quest != null)
			ChangeQuestStatus();
	}

	public void ChangeQuestStatus()
	{
		if (questTrigger)
			quest.Trigger();
		if (questCompleted)
			quest.Complete();
		if (questFailed)
			quest.Fail();
	}




}
