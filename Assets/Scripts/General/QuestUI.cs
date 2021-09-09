using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;

public class QuestUI : MonoBehaviour
{
	public UIDocument UIDocument;
	VisualElement questUI;
	VisualElement activeQuestsElement;
	VisualElement completedQuestsElement;
	VisualElement failedQuestsElement;
	VisualElement questInformation;

	public InputMaster controls;

	GameObject player;

	QuestManager questManager;


	void Awake()
	{
		var root = UIDocument.rootVisualElement;
		questUI = root.Q<VisualElement>("questUI");
		activeQuestsElement = root.Q<VisualElement>("activeQuests");
		completedQuestsElement = root.Q<VisualElement>("completedQuests");
		failedQuestsElement = root.Q<VisualElement>("failedQuests");
		questInformation = root.Q<VisualElement>("questInformation");

		controls = new InputMaster();
		controls.FMPlayer.EnableQuestUI.performed += ctx => EnableQuestUI();

		controls.UI.Test.performed += ctx => Test();
		controls.UI.DisableQuestUI.performed += ctx => DisableQuestUI();

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
		PauseGame();

		controls.UI.Enable();
	}

	void DisableQuestUI()
	{
		questUI.style.display = DisplayStyle.None;

		controls.FMPlayer.Enable();
		ResumeGame();

		controls.UI.Disable();
	}

	void PopulateQuestUI()
	{
		List<Quest> activeQuests = questManager.ReturnActiveQuests();
		foreach (Quest quest in activeQuests)
		{
			Label questName = new Label(quest.qName);
			// https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
			// myElement.RegisterCallback<MouseDownEvent, MyType>(MyCallbackWithData, myData);
			// void MyCallbackWithData(MouseDownEvent evt, MyType data) { /* ... */ }
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			activeQuestsElement.Add(questName);
		}

		List<Quest> completedQuests = questManager.ReturnCompletedQuests();
		foreach (Quest quest in completedQuests)
		{
			Label questName = new Label(quest.qName);
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			completedQuestsElement.Add(questName);
		}

		List<Quest> failedQuests = questManager.ReturnFailedQuests();
		foreach (Quest quest in failedQuests)
		{
			Label questName = new Label(quest.qName);
			questName.RegisterCallback<MouseDownEvent, Quest>(DisplayQuestInformation, quest);
			failedQuestsElement.Add(questName);
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

	void PauseGame()
	{
		player.SetActive(false);
		Time.timeScale = 0;
	}

	void ResumeGame()
	{
		player.SetActive(true);
		Time.timeScale = 1;
	}

}
