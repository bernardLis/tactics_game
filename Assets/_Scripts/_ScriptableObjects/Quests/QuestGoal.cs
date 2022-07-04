using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestGoalState { INACTIVE, ACTIVE, COMPLETED, FAILED }

public abstract class QuestGoal : ScriptableObject
{
	public string Title;
	public QuestGoalState QuestGoalState;

	// TODO: required item is a smart idea, I need to implement it well tho.
	public Item RequiredItem;
	public int CurrentAmount;
	public int RequiredAmount;

	public abstract void Initialize();
	public abstract void Evaluate();
	public abstract void Evaluate(object sender, ItemChangedEventArgs e);
	public abstract void Complete();
	public abstract void CleanUp();
	
}
