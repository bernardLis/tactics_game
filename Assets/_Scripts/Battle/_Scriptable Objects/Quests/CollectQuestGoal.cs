using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Quests/Goals/Collect Quest Goal")]
public class CollectQuestGoal : QuestGoal
{
	public override void Initialize()
	{
		// TODO: this writes over the values that were saved from previous 'test play'
		CurrentAmount = 0;
		QuestGoalState = QuestGoalState.ACTIVE;

		// check if hero has the item in the inventory already and update current amount & evaluate
		foreach (Item item in InventoryManager.Instance.items)
			if (item == RequiredItem)
				CurrentAmount++;

		// subscribe to on item changed by Inventory.cs
		// TODO: this is hacky/wrong... but I need to make sure evaluate is subscribed only once. 
		InventoryManager.Instance.OnItemChanged -= Evaluate;
		InventoryManager.Instance.OnItemChanged += Evaluate;
		Evaluate();
	}

	public override void Evaluate()
	{
		// TODO: I am not certain about this logic here.
		if (CurrentAmount >= RequiredAmount && QuestGoalState != QuestGoalState.COMPLETED)
			Complete();
		else if (CurrentAmount < RequiredAmount)
			QuestGoalState = QuestGoalState.ACTIVE;
	}

	public override void Evaluate(object sender, ItemChangedEventArgs e)
	{
		if (e.Item == RequiredItem)
		{
			CurrentAmount++;
			GameUI.Instance.DisplayLogText(CurrentAmount + "/" + RequiredAmount + " of " + e.Item.name);
			Evaluate();
		}
	}

	public override void Complete()
	{
		GameUI.Instance.DisplayLogText("Quest Goal compelted! " + Title);

		QuestGoalState = QuestGoalState.COMPLETED;
	}

	public override void CleanUp()
	{
		InventoryManager.Instance.OnItemChanged -= Evaluate;

		// on quest complete remove items from the inventory
		if (RequiredItem != null)
			for (var i = 0; i < RequiredAmount; i++)
				InventoryManager.Instance.Remove(RequiredItem);

	}

}
