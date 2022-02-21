using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestGoalState { INACTIVE, ACTIVE, COMPLETED, FAILED }

public abstract class QuestGoal : ScriptableObject
{
	public string title;
	public QuestGoalState qGoalState;

	// TODO: required item is a smart idea, I need to implement it well tho.
	public Item requiredItem;
	public int currentAmount;
	public int requiredAmount;

	public abstract void Initialize();
	public abstract void Evaluate();
	public abstract void Evaluate(object sender, ItemChangedEventArgs e);
	public abstract void Complete();
	public abstract void CleanUp();
	
}
