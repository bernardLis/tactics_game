using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    // TODO: refactor this more 
    List<QuestSlot> _questSlots = new();

    UIDocument _UIDocument;
    VisualElement _questContainer;
    VisualElement _activeQuestsContainer;
    VisualElement _completedQuestsContainer;
    VisualElement _failedQuestsContainer;
    VisualElement _questInformation;
    VisualElement _questGoalContainer;

    PlayerInput _playerInput;

    QuestManager _questManager;

    [SerializeField] Texture2D _checkmark;
    public QuestSlot SelectedQuestSlot;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _questManager = GetComponent<QuestManager>();

        _UIDocument = GetComponent<UIDocument>();
        var root = _UIDocument.rootVisualElement;

        _questContainer = root.Q<VisualElement>("questContainer");
        _activeQuestsContainer = root.Q<VisualElement>("activeQuestsContainer");
        _completedQuestsContainer = root.Q<VisualElement>("completedQuestsContainer");
        _failedQuestsContainer = root.Q<VisualElement>("failedQuestsContainer");
        _questInformation = root.Q<VisualElement>("questInformation");

    }

    void OnEnable()
    {
        _playerInput.actions["DisableQuestUI"].performed += ctx => DisableQuestUI();
        _playerInput.actions["QuestMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());

        ClearQuestInformation();
    }

    void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.actions["DisableQuestUI"].performed -= ctx => DisableQuestUI();
            _playerInput.actions["QuestMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());
        }
    }

    void Move(Vector2 direction)
    {
        // selectedSlot to be overwritten;
        QuestSlot slot = null;
        // https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list
        // if it is right - select next slot
        if (direction.Equals(Vector2.down))
            slot = _questSlots.SkipWhile(x => x != SelectedQuestSlot).Skip(1).DefaultIfEmpty(_questSlots[0]).FirstOrDefault();
        if (direction.Equals(Vector2.up))
            slot = _questSlots.TakeWhile(x => x != SelectedQuestSlot).DefaultIfEmpty(_questSlots[_questSlots.Count - 1]).LastOrDefault();

        // not interaction for left and right;
        if (slot == null)
            return;

        slot.Select();
        //inventorySlotContainer.ScrollTo(slot);
    }


    public void EnableQuestUI()
    {
        // switch action map
        _playerInput.SwitchCurrentActionMap("QuestUI");
        BattleManager.Instance.PauseGame();

        PopulateQuestUI();

        // refresh quest info if player had it open previously
        if (SelectedQuestSlot != null)
            RefreshQuestInformation();

        // only one can be visible/
        //GameUI.Instance.HideAllUIPanels();

        _questContainer.style.display = DisplayStyle.Flex;
    }

    public void DisableQuestUI()
    {
        _questContainer.style.display = DisplayStyle.None;

        // BattleManager.Instance.EnableFMPlayerControls(); << TODO: decide when is quest ui / inventory ui accessible
        BattleManager.Instance.ResumeGame();
    }

    void PopulateQuestUI()
    {
        _questSlots.Clear();

        _activeQuestsContainer.Clear();
        _completedQuestsContainer.Clear();
        _failedQuestsContainer.Clear();

        List<Quest> activeQuests = _questManager.ReturnActiveQuests();
        List<Quest> completedQuests = _questManager.ReturnCompletedQuests();
        List<Quest> failedQuests = _questManager.ReturnFailedQuests();

        if (activeQuests.Count != 0)
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("activeQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in activeQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                _activeQuestsContainer.Add(slot);

                _questSlots.Add(slot);
            }
        }
        else
            _UIDocument.rootVisualElement.Q<VisualElement>("activeQuests").style.display = DisplayStyle.None;

        if (completedQuests.Count != 0)
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in completedQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                _completedQuestsContainer.Add(slot);

                _questSlots.Add(slot);
            }
        }
        else
            _UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.None;


        if (failedQuests.Count != 0)
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.Flex;

            foreach (Quest quest in failedQuests)
            {
                QuestSlot slot = new QuestSlot();
                slot.HoldQuest(quest);
                _failedQuestsContainer.Add(slot);

                _questSlots.Add(slot);
            }

        }
        else
            _UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.None;

        // the list has changed I need to update selected quest slot
        if (SelectedQuestSlot != null)
        {
            foreach (QuestSlot slot in _questSlots)
            {
                if (slot.Quest == SelectedQuestSlot.Quest)
                {
                    SelectedQuestSlot = slot;
                }
            }
        }

        // if there are no quests to display add a text;
        if (activeQuests.Count == 0 && completedQuests.Count == 0 && failedQuests.Count == 0)
        {
            ClearQuestInformation();
            Label noQuests = new Label("Such empty, get some quests!");
            noQuests.AddToClassList("questTitleLabel");
            _questInformation.Add(noQuests);
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
        if (SelectedQuestSlot != null)
            SelectedQuestSlot.Unselect();
    }

    void DisplayQuestInformation(Quest quest)
    {
        // TODO: show that the quest is completed

        // TODO: could be even nicer;
        Label questName = new Label(quest.Title);
        questName.AddToClassList("questInformationName");
        _questInformation.Add(questName);

        Label questDescription = new Label(quest.Description);
        questDescription.AddToClassList("questInformationDescription");
        _questInformation.Add(questDescription);

        VisualElement questRewardContainer = new VisualElement();
        questRewardContainer.AddToClassList("questCurrentRequiredContainer");
        _questInformation.Add(questRewardContainer);

        // quest reward icon
        if (quest.Reward != null)
        {
            Label questRewardLabel = new Label("Reward: ");
            questRewardLabel.AddToClassList("questGoalLabels");
            Label questReward = new Label();
            questReward.AddToClassList("questItem");
            questReward.style.backgroundImage = quest.Reward.Icon.texture;
            questRewardContainer.Add(questRewardLabel);
            questRewardContainer.Add(questReward);
        }

        foreach (QuestGoal questGoal in quest.Goals)
        {
            VisualElement questGoalContainer = new VisualElement();
            questGoalContainer.AddToClassList("questGoalContainer");
            _questInformation.Add(questGoalContainer);

            Label title = new Label("Goal: " + questGoal.Title);
            title.AddToClassList("questGoalTitle");
            questGoalContainer.Add(title);

            if (questGoal.QuestGoalState == QuestGoalState.COMPLETED)
            {
                questGoalContainer.style.backgroundImage = _checkmark;
                questGoalContainer.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            }

            if (questGoal.RequiredItem == null)
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
            for (int i = 0; i < questGoal.CurrentAmount; i++)
            {
                Label item = new Label();
                item.style.backgroundImage = questGoal.RequiredItem.Icon.texture;
                item.AddToClassList("questItem");
                currentContainer.Add(item);
            }

            for (int i = 0; i < questGoal.RequiredAmount; i++)
            {
                Label item = new Label();
                item.style.backgroundImage = questGoal.RequiredItem.Icon.texture;
                item.AddToClassList("questItem");
                requiredContainer.Add(item);
            }
        }

    }

    void RefreshQuestInformation()
    {
        ClearQuestInformation();
        if (SelectedQuestSlot != null)
        {
            // DisplayQuestInformation(selectedQuestSlot.quest);
            SelectedQuestSlot.Select();
        }
    }

    void ClearQuestInformation()
    {
        _questInformation.Clear();
    }

    void Test()
    {
        print("testtest");
    }
}
