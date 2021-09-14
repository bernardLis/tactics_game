using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestState { INACTIVE, ACTIVE, COMPLETED, FAILED }

public abstract class Quest : ScriptableObject
{
	public int qID;
	public string qName;
	public string qDescription;
	public QuestState qState;
	public QuestGoal[] qGoals;
	public Item qReward;

	public abstract void Trigger();
	public abstract void Complete();
	public abstract void Fail();

}
