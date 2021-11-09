using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    List<QuestSlot> questSlots = new();

    UIDocument UIDocument;
    VisualElement questUI;
    VisualElement activeQuestsContainer;
    VisualElement completedQuestsContainer;
    VisualElement failedQuestsContainer;
    VisualElement questInformation;
    VisualElement questGoalContainer;

    GameObject player;
    PlayerInput playerInput;

    QuestManager questManager;

    public Texture2D checkmark;
    public QuestSlot selectedQuestSlot;

    void Awake()
    {
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        questUI = root.Q<VisualElement>("questUI");
        activeQuestsContainer = root.Q<VisualElement>("activeQuestsContainer");
        completedQuestsContainer = root.Q<VisualElement>("completedQuestsContainer");
        failedQuestsContainer = root.Q<VisualElement>("failedQuestsContainer");
        questInformation = root.Q<VisualElement>("questInformation");

        playerInput = MovePointController.instance.GetComponent<PlayerInput>();

        questManager = GameManager.instance.GetComponent<QuestManager>();
    }

    void OnEnable()
    {
        playerInput.actions["DisableQuestUI"].performed += ctx => DisableQuestUI();
        playerInput.actions["QuestMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());

        ClearQuestInformation();
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["DisableQuestUI"].performed -= ctx => DisableQuestUI();
            playerInput.actions["QuestMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());
        }
    }

    void Move(Vector2 direction)
    {
        // selectedSlot to be overwritten;
        QuestSlot slot = null;
        // https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list
        // if it is right - select next slot
        if (direction.Equals(Vector2.down))
            slot = questSlots.SkipWhile(x => x != selectedQuestSlot).Skip(1).DefaultIfEmpty(questSlots[0]).FirstOrDefault();
        if (direction.Equals(Vector2.up))
            slot = questSlots.TakeWhile(x => x != selectedQuestSlot).DefaultIfEmpty(questSlots[questSlots.Count - 1]).LastOrDefault();

        // not interaction for left and right;
        if (slot == null)
            return;

        slot.Select();
        //inventorySlotContainer.ScrollTo(slot);
    }


    public void EnableQuestUI()
    {
        // switch action map
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("QuestUI");
        GameManager.instance.PauseGame();

        PopulateQuestUI();

        // refresh quest info if player had it open previously
        if (selectedQuestSlot != null)
            RefreshQuestInformation();

        // only one can be visible/
        GameUI.instance.HideAllUIPanels();

        questUI.style.display = DisplayStyle.Flex;
    }

    public void DisableQuestUI()
    {
        questUI.style.display = DisplayStyle.None;

        GameManager.instance.EnableFMPlayerControls();
        GameManager.instance.ResumeGame();
    }

    void PopulateQuestUI()
    {
        questSlots.Clear();

        activeQuestsContainer.Clear();
        completedQuestsContainer.Clear();
        failedQuestsContainer.Clear();

        List<Quest> activeQuests = questManager.ReturnActiveQuests();
        List<Quest> completedQuests = questManager.ReturnCompletedQuests();
        List<Quest> failedQuests = questManager.ReturnFailedQuests();

        if (activeQuests.Count != 0)
        {
            UIDocument.rootVisualElement.Q<VisualElement>("activeQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in activeQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                activeQuestsContainer.Add(slot);

                questSlots.Add(slot);
            }
        }
        else
            UIDocument.rootVisualElement.Q<VisualElement>("activeQuests").style.display = DisplayStyle.None;

        if (completedQuests.Count != 0)
        {
            UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in completedQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                completedQuestsContainer.Add(slot);

                questSlots.Add(slot);
            }
        }
        else
            UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.None;


        if (failedQuests.Count != 0)
        {
            UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in failedQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                failedQuestsContainer.Add(slot);

                questSlots.Add(slot);
            }

        }
        else
            UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.None;

        // the list has changed I need to update selected quest slot
        if (selectedQuestSlot != null)
        {
            foreach (QuestSlot slot in questSlots)
            {
                if (slot.quest == selectedQuestSlot.quest)
                {
                    Debug.Log("selected questtt found");
                    selectedQuestSlot = slot;
                }
            }
        }

        // if there are no quests to display add a text;
        if (activeQuests.Count == 0 && completedQuests.Count == 0 && failedQuests.Count == 0)
        {
            ClearQuestInformation();
            Label noQuests = new Label("Such empty, get some quests!");
            noQuests.AddToClassList("questTitleLabel");
            questInformation.Add(noQuests);
        }
        else
            RefreshQuestInformation();
    }

    public void OnQuestClick(Quest quest)
    {

        ClearQuestInformation();
        DisplayQuestInformation(quest);
    }

    public void UnselectCurrent()
    {
        if (selectedQuestSlot != null)
            selectedQuestSlot.Unselect();
    }

    void DisplayQuestInformation(Quest quest)
    {
        // TODO: show that the quest is completed

        // TODO: could be even nicer;
        Label questName = new Label(quest.qName);
        questName.AddToClassList("questInformationName");
        questInformation.Add(questName);

        Label questDescription = new Label(quest.qDescription);
        questDescription.AddToClassList("questInformationDescription");
        questInformation.Add(questDescription);

        VisualElement questRewardContainer = new VisualElement();
        questRewardContainer.AddToClassList("questCurrentRequiredContainer");
        questInformation.Add(questRewardContainer);

        // quest reward icon
        if (quest.qReward != null)
        {
            Label questRewardLabel = new Label("Reward: ");
            questRewardLabel.AddToClassList("questGoalLabels");
            Label questReward = new Label();
            questReward.AddToClassList("questItem");
            questReward.style.backgroundImage = quest.qReward.icon.texture;
            questRewardContainer.Add(questRewardLabel);
            questRewardContainer.Add(questReward);
        }

        foreach (QuestGoal questGoal in quest.qGoals)
        {
            VisualElement questGoalContainer = new VisualElement();
            questGoalContainer.AddToClassList("questGoalContainer");
            questInformation.Add(questGoalContainer);

            Label title = new Label("Goal: " + questGoal.title);
            title.AddToClassList("questGoalTitle");
            questGoalContainer.Add(title);

            if (questGoal.qGoalState == QuestGoalState.COMPLETED)
            {
                questGoalContainer.style.backgroundImage = checkmark;
                questGoalContainer.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }

            if (questGoal.requiredItem == null)
                return;

            VisualElement currentContainer = new VisualElement();
            VisualElement requiredContainer = new VisualElement();

            currentContainer.AddToClassList("questCurrentRequiredContainer");
            requiredContainer.AddToClassList("questCurrentRequiredContainer");

            questGoalContainer.Add(currentContainer);
            questGoalContainer.Add(requiredContainer);

            Label currentLabel = new Label("Owned:");
            Label requiredLabel = new Label("Required:");
            currentLabel.AddToClassList("questGoalLabels");
            requiredLabel.AddToClassList("questGoalLabels");
            currentContainer.Add(currentLabel);
            requiredContainer.Add(requiredLabel);

            // items
            for (int i = 0; i < questGoal.currentAmount; i++)
            {
                Label item = new Label();
                item.style.backgroundImage = questGoal.requiredItem.icon.texture;
                item.AddToClassList("questItem");
                currentContainer.Add(item);
            }

            for (int i = 0; i < questGoal.requiredAmount; i++)
            {
                Label item = new Label();
                item.style.backgroundImage = questGoal.requiredItem.icon.texture;
                item.AddToClassList("questItem");
                requiredContainer.Add(item);
            }
        }

    }

    void RefreshQuestInformation()
    {
        ClearQuestInformation();
        if (selectedQuestSlot != null)
        {
            // DisplayQuestInformation(selectedQuestSlot.quest);
            selectedQuestSlot.Select();
        }
    }

    void ClearQuestInformation()
    {
        questInformation.Clear();
    }

    void Test()
    {
        print("testtest");
    }
}
