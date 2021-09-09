using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestGoal : ScriptableObject
{
	public string title;
	public bool completed;
	// TODO: required item is a smart idea, I need to implement it well tho.
	public ScriptableObject requiredItem;
	public int currentAmount;
	public int requiredAmount;

	public abstract void Evaluate();
	public abstract void Complete();
}
