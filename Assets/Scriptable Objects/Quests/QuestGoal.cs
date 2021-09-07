using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestGoal : ScriptableObject
{
	public string title;
	public bool completed;
	public int currentAmount;
	public int requiredAmount;

	public abstract void Evaluate();
	public abstract void Complete();
}
