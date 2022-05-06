using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Quests/Goals/Collect Quest Goal")]
public class CollectQuestGoal : QuestGoal
{
    InventoryManager _inventoryManager;

    public override void Initialize()
    {
        _inventoryManager = GameManager.Instance.GetComponent<InventoryManager>();

        // TODO: this writes over the values that were saved from previous 'test play'
        CurrentAmount = 0;
        QuestGoalState = QuestGoalState.ACTIVE;

        // check if hero has the item in the inventory already and update current amount & evaluate
        foreach (Item item in _inventoryManager.items)
            if (item == RequiredItem)
                CurrentAmount++;

        // subscribe to on item changed by Inventory.cs
        // TODO: this is hacky/wrong... but I need to make sure evaluate is subscribed only once. 
        _inventoryManager.OnItemChanged -= Evaluate;
        _inventoryManager.OnItemChanged += Evaluate;
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
            Evaluate();
        }
    }

    public override void Complete()
    {
        QuestGoalState = QuestGoalState.COMPLETED;
    }

    public override void CleanUp()
    {
        _inventoryManager.OnItemChanged -= Evaluate;

        // on quest complete remove items from the inventory
        if (RequiredItem != null)
            for (var i = 0; i < RequiredAmount; i++)
                _inventoryManager.Remove(RequiredItem);
    }

}
