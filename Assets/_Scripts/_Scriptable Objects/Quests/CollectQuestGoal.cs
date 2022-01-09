using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quests/Goals/Collect Quest Goal")]
public class CollectQuestGoal : QuestGoal
{

	public override void Initialize()
	{
		// TODO: this writes over the values that were saved from previous 'test play'
		currentAmount = 0;
		qGoalState = QuestGoalState.ACTIVE;

		// check if hero has the item in the inventory already and update current amount & evaluate
		foreach (Item item in Inventory.instance.items)
		{
			if (item == requiredItem)
			{
				currentAmount++;
			}
		}

		// subscribe to on item changed by Inventory.cs
		// TODO: this is hacky/wrong... but I need to make sure evaluate is subscribed only once. 
		Inventory.instance.OnItemChanged -= Evaluate;
		Inventory.instance.OnItemChanged += Evaluate;
		Evaluate();
	}

	public override void Evaluate()
	{
		// TODO: I am not certain about this logic here.
		if (currentAmount >= requiredAmount && qGoalState != QuestGoalState.COMPLETED)
		{
			Complete();
		}
		else if (currentAmount < requiredAmount)
		{
			qGoalState = QuestGoalState.ACTIVE;
		}
	}

	public override void Evaluate(object sender, ItemChangedEventArgs e)
	{
		Debug.Log("evaluate with object etc. is called");
		if (e.item == requiredItem)
		{
			currentAmount++;
			GameUI.instance.DisplayLogText(currentAmount + "/" + requiredAmount + " of " + e.item.iName);
			Evaluate();
		}
	}

	public override void Complete()
	{
		GameUI.instance.DisplayLogText("Quest Goal compelted! " + title);

		Debug.Log("quest goal compelte");
		qGoalState = QuestGoalState.COMPLETED;
	}

	public override void CleanUp()
	{
		Inventory.instance.OnItemChanged -= Evaluate;

		// on quest complete remove items from the inventory
		if (requiredItem != null)
		{
			for (var i = 0; i < requiredAmount; i++)
			{
				Inventory.instance.Remove(requiredItem);
			}
		}
	}

}
