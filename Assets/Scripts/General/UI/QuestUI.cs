using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
	UIDocument UIDocument;
	VisualElement questUI;
	VisualElement activeQuestsContainer;
	VisualElement completedQuestsContainer;
	VisualElement failedQuestsContainer;
	VisualElement questInformation;

	public InputMaster controls;

	GameObject player;

	QuestManager questManager;

	void Awake()
	{
		UIDocument = GetComponent<UIDocument>();
		var root = UIDocument.rootVisualElement;

		questUI = root.Q<VisualElement>("questUI");
		activeQuestsContainer = root.Q<VisualElement>("activeQuestsContainer");
		completedQuestsContainer = root.Q<VisualElement>("completedQuestsContainer");
		failedQuestsContainer = root.Q<VisualElement>("failedQuestsContainer");
		questInformation = root.Q<VisualElement>("questInformation");

		controls = new InputMaster();
		controls.FMPlayer.EnableQuestUI.performed += ctx => EnableQuestUI();

		controls.QuestUI.Test.performed += ctx => Test();
		controls.QuestUI.DisableQuestUI.performed += ctx => DisableQuestUI();

		player = GameObject.FindGameObjectWithTag("Player");

		questManager = GameManager.instance.GetComponent<QuestManager>();
	}

	void OnEnable()
	{
		controls.FMPlayer.Enable();
	}

	void OnDisable()
	{
		controls.FMPlayer.Disable();
	}

	void EnableQuestUI()
	{
		PopulateQuestUI();
		questUI.style.display = DisplayStyle.Flex;
		// TODO: only controls.FMPlayer.Disable() does not disable player controlls
		controls.FMPlayer.Disable();
		player.SetActive(false);
		GameManager.instance.PauseGame();

		controls.QuestUI.Enable();
	}

	void DisableQuestUI()
	{
		questUI.style.display = DisplayStyle.None;

		controls.FMPlayer.Enable();
		player.SetActive(true);
		GameManager.instance.ResumeGame();

		controls.QuestUI.Disable();
	}

	void PopulateQuestUI()
	{

		activeQuestsContainer.Clear();
		completedQuestsContainer.Clear();
		failedQuestsContainer.Clear();

		List<Quest> activeQuests = questManager.ReturnActiveQuests();
		foreach (Quest quest in activeQuests)
		{
			Label questName = new Label(quest.qName);
			// https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
			// myElement.RegisterCallback<MouseDownEvent, MyType>(MyCallbackWithData, myData);
			// void MyCallbackWithData(MouseDownEvent evt, MyType data) { /* ... */ }
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			activeQuestsContainer.Add(questName);
		}

		List<Quest> completedQuests = questManager.ReturnCompletedQuests();
		foreach (Quest quest in completedQuests)
		{
			Label questName = new Label(quest.qName);
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			completedQuestsContainer.Add(questName);
		}

		List<Quest> failedQuests = questManager.ReturnFailedQuests();
		foreach (Quest quest in failedQuests)
		{
			Label questName = new Label(quest.qName);
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			failedQuestsContainer.Add(questName);
		}
	}

	void DisplayQuestInformation(MouseDownEvent evt, Quest quest)
	{
		ClearQuestInformation();

		Label questName = new Label(quest.qName);
		questInformation.Add(questName);
		Label questDescription = new Label(quest.qDescription);
		questInformation.Add(questDescription);

		foreach (QuestGoal questGoal in quest.qGoals)
		{
			print("in q goal" + questGoal.title);
			Label questGoalLabel = new Label(questGoal.title);
			questInformation.Add(questGoalLabel);
			// TODO: display item required 
			// Label itemRequired = new Label(questGoal.requiredItem)
			Label amountRequired = new Label(questGoal.currentAmount + "/" + questGoal.requiredAmount);
			questInformation.Add(amountRequired);
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
