using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestState { INACTIVE, ACTIVE, COMPLETED, FAILED }

public abstract class Quest : ScriptableObject
{
	public string qName;
	public string qDescription;
	public QuestState qState;
	public QuestGoal[] qGoals;
	public ScriptableObject qReward;

	public abstract void TriggerQuest();
	public abstract void CompleteQuest();
	public abstract void FailQuest();

}
