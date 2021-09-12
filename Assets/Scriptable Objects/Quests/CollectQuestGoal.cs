using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quests/Goals/Collect Quest Goal")]
public class CollectQuestGoal : QuestGoal
{

	public override void Initialize()
	{
		// subscribe to on item changed by Inventory.cs
		Inventory.instance.OnItemChanged += Evaluate;
	}

	public override void Evaluate()
	{
		if (currentAmount >= requiredAmount)
		{
			Complete();
		}
	}

	public override void Evaluate(object sender, ItemChangedEventArgs e)
	{
		if (e.item == requiredItem)
		{
			currentAmount++;
			GameUI.instance.DisplayLogText(currentAmount + "/" + requiredAmount + " of " + e.item.iName);
			Evaluate();
		}
	}

	public override void Complete()
	{
		Inventory.instance.OnItemChanged -= Evaluate;
		// TODO:
		Debug.Log("quest goal compelte");
	}
}
