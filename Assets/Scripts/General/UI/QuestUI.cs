using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
	UIDocument UIDocument;
	VisualElement inventoryContainer;
	VisualElement questUI;
	VisualElement activeQuestsContainer;
	VisualElement completedQuestsContainer;
	VisualElement failedQuestsContainer;
	VisualElement questInformation;
	VisualElement questGoalContainer;

	GameObject player;
	PlayerInput playerInput;

	QuestManager questManager;

	Quest lastClickedQuest;

	void Awake()
	{
		UIDocument = GetComponent<UIDocument>();
		var root = UIDocument.rootVisualElement;

		inventoryContainer = root.Q<VisualElement>("inventoryContainer");

		questUI = root.Q<VisualElement>("questUI");
		activeQuestsContainer = root.Q<VisualElement>("activeQuestsContainer");
		completedQuestsContainer = root.Q<VisualElement>("completedQuestsContainer");
		failedQuestsContainer = root.Q<VisualElement>("failedQuestsContainer");
		questInformation = root.Q<VisualElement>("questInformation");

		player = GameObject.FindGameObjectWithTag("Player");
		playerInput = player.GetComponent<PlayerInput>();

		questManager = GameManager.instance.GetComponent<QuestManager>();
	}

	void OnEnable()
	{
		playerInput.actions["DisableQuestUI"].performed += ctx => DisableQuestUI();

		ClearQuestInformation();
	}

	void OnDisable()
	{
		playerInput.actions["DisableQuestUI"].performed -= ctx => DisableQuestUI();
	}

	public void EnableQuestUI()
	{
		// switch action map
		player.GetComponent<PlayerInput>().SwitchCurrentActionMap("QuestUI");
		GameManager.instance.PauseGame();

		PopulateQuestUI();

		// refresh quest info if player had it open previously
		if (lastClickedQuest != null)
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
				Label questLabel = new Label(quest.qName);
				questLabel.AddToClassList("questTitleLabel");
				// https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
				// myElement.RegisterCallback<MouseDownEvent, MyType>(MyCallbackWithData, myData);
				// void MyCallbackWithData(MouseDownEvent evt, MyType data) { /* ... */ }
				questLabel.RegisterCallback<MouseDownEvent, Quest>(OnQuestClick, quest);
				activeQuestsContainer.Add(questLabel);
			}
		}
		else
			UIDocument.rootVisualElement.Q<VisualElement>("activeQuests").style.display = DisplayStyle.None;

		if (completedQuests.Count != 0)
		{
			UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.Flex;

			foreach (Quest quest in completedQuests)
			{
				Label questLabel = new Label(quest.qName);
				questLabel.AddToClassList("questTitleLabel");

				questLabel.RegisterCallback<MouseDownEvent, Quest>(OnQuestClick, quest);
				completedQuestsContainer.Add(questLabel);
			}
		}
		else
			UIDocument.rootVisualElement.Q<VisualElement>("completedQuests").style.display = DisplayStyle.None;


		if (failedQuests.Count != 0)
		{
			UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.Flex;

			foreach (Quest quest in failedQuests)
			{
				Label questLabel = new Label(quest.qName);
				questLabel.AddToClassList("questTitleLabel");

				questLabel.RegisterCallback<MouseDownEvent, Quest>(OnQuestClick, quest);
				failedQuestsContainer.Add(questLabel);
			}
		}
		else
			UIDocument.rootVisualElement.Q<VisualElement>("failedQuests").style.display = DisplayStyle.None;

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

	void OnQuestClick(MouseDownEvent evt, Quest quest)
	{
		lastClickedQuest = quest;
		ClearQuestInformation();
		DisplayQuestInformation(quest);
	}

	void DisplayQuestInformation(Quest quest)
	{
		// show that the quest is completed

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
			if(questGoal.requiredItem == null)
				return;

			VisualElement questGoalContainer = new VisualElement();
			questInformation.Add(questGoalContainer);

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
		if (lastClickedQuest != null)
			DisplayQuestInformation(lastClickedQuest);
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
